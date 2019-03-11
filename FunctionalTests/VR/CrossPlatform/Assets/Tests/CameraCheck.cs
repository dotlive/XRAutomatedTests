using UnityEngine;
using UnityEngine.XR;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using System.IO;

public class CameraCheck : TestBaseSetup
{
    private bool m_RaycastHit = false;
    private bool m_DidSaveScreenCapture = false;
    private string m_FileName;

    private float m_StartingScale;
    private float m_StartingZoomAmount;
    private float m_StartingRenderScale;
    private float kDeviceSetupWait = 1f;
    
    private Texture2D m_MobileTexture;

    void Start()
    {
        m_StartingScale = XRSettings.eyeTextureResolutionScale;
        m_StartingZoomAmount = XRDevice.fovZoomFactor;
        m_StartingRenderScale = XRSettings.renderViewportScale;
    }

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        m_TestSetupHelpers.TestCubeSetup(TestCubesConfig.TestCube);
    }

    [TearDown]
    public override void TearDown()
    {
        m_RaycastHit = false;

        XRSettings.eyeTextureResolutionScale = 1f;
        XRDevice.fovZoomFactor = m_StartingZoomAmount;
        XRSettings.renderViewportScale = 1f;

        base.TearDown();
    }

#if UNITY_EDITOR
    [Ignore("Known bug")]
    [UnityTest]
    public IEnumerator CameraCheckForMultiPass()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.MultiPass);
        Assert.AreEqual(XRSettings.stereoRenderingMode, UnityEditor.PlayerSettings.stereoRenderingPath, "Expected StereoRenderingPath to be Multi pass");
    }

    [Ignore("Known bug")]
    [UnityTest]
    public IEnumerator CameraCheckForInstancing()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        m_TestSetupHelpers.TestStageSetup(TestStageConfig.Instancing);
        Assert.AreEqual(XRSettings.stereoRenderingMode, UnityEditor.PlayerSettings.stereoRenderingPath, "Expected StereoRenderingPath to be Instancing");
    }
#endif

    [UnityTest]
    public IEnumerator CheckRefreshRate()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        var refreshRate = XRDevice.refreshRate;
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            Assert.GreaterOrEqual(refreshRate, 60, "Refresh rate returned to lower than expected");
        }
        if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            Assert.GreaterOrEqual(refreshRate, 89, "Refresh rate returned to lower than expected");
        }
    }

    [UnityTest]
    public IEnumerator RenderViewportScale()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        XRSettings.renderViewportScale = 1f;
        Assert.AreEqual(1f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");

        XRSettings.renderViewportScale = 0.7f;
        Assert.AreEqual(0.7f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");

        XRSettings.renderViewportScale = 0.5f;
        Assert.AreEqual(0.5f, XRSettings.renderViewportScale, "Render viewport scale is not being respected");
    }


    [UnityTest]
    public IEnumerator EyeTextureResolutionScale()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        float scale = 0.1f;
        float scaleCount = 0.1f;

        for (float i = 0.1f; i < 2; i++)
        {
            scale = scale + 0.1f;
            scaleCount = scaleCount + 0.1f;

            XRSettings.eyeTextureResolutionScale = scale;

            yield return null;

            Debug.Log("EyeTextureResolutionScale = " + scale);
            Assert.AreEqual(scaleCount, XRSettings.eyeTextureResolutionScale, "Eye texture resolution scale is not being respected");
        }
    }

    [UnityTest]
    public IEnumerator DeviceZoom()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        float zoomAmount = 0f;
        float zoomCount = 0f;

        for (int i = 0; i < 2; i++)
        {
            zoomAmount = zoomAmount + 1f;
            zoomCount = zoomCount + 1f;

            XRDevice.fovZoomFactor = zoomAmount;

            yield return null;

            Debug.Log("fovZoomFactor = " + zoomAmount);
            Assert.AreEqual(zoomCount, XRDevice.fovZoomFactor, "Zoom Factor is not being respected");
        }
    }

    [UnityTest]
    public IEnumerator TakeScreenShot()
    {
        yield return new WaitForSeconds(kDeviceSetupWait);

        try
        {
            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                var cam = GameObject.Find("Camera");
                var width = cam.GetComponent<Camera>().scaledPixelWidth;
                var height = cam.GetComponent<Camera>().scaledPixelHeight;

                m_MobileTexture  = new Texture2D(width, height, TextureFormat.RGBA32, false);
                m_MobileTexture = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
            }
            else
            {
                m_FileName = Application.temporaryCachePath + "/ScreenShotTest.jpg";
                ScreenCapture.CaptureScreenshot(m_FileName, ScreenCapture.StereoScreenCaptureMode.BothEyes);
            }

            m_DidSaveScreenCapture = true;
        }
        catch (Exception e)
        {
            Debug.Log("Failed to get capture! : " + e);
            m_DidSaveScreenCapture = false;
            Assert.Fail("Failed to get capture! : " + e);
        }

        if (m_DidSaveScreenCapture && Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            yield return new WaitForSeconds(5);

            if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Assert.IsNotNull(m_MobileTexture, "Texture data is empty for mobile");
            }
            else
            {
                var tex = new Texture2D(2, 2);

                var texData = File.ReadAllBytes(m_FileName);
                Debug.Log("Screen Shot Success!" + Environment.NewLine + "File Name = " + m_FileName);

                tex.LoadImage(texData);

                Assert.IsNotNull(tex, "Texture Data is empty");
            }
        }
    }
}
