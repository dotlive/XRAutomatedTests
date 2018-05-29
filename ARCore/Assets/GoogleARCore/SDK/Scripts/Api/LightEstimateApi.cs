//-----------------------------------------------------------------------
// <copyright file="LightEstimateApi.cs" company="Google">
//
// Copyright 2017 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCoreInternal
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;
    using GoogleARCore;

    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
         Justification = "Internal")]
    public class LightEstimateApi
    {
        private NativeApi m_NativeApi;

        public LightEstimateApi(NativeApi nativeApi)
        {
            m_NativeApi = nativeApi;
        }

        public IntPtr Create()
        {
            IntPtr lightEstimateHandle = IntPtr.Zero;
            ExternApi.ArLightEstimate_create(m_NativeApi.SessionHandle, ref lightEstimateHandle);
            return lightEstimateHandle;
        }

        public void Destroy(IntPtr lightEstimateHandle)
        {
            ExternApi.ArLightEstimate_destroy(lightEstimateHandle);
        }

        public LightEstimateState GetState(IntPtr lightEstimateHandle)
        {
            ApiLightEstimateState state = ApiLightEstimateState.NotValid;
            ExternApi.ArLightEstimate_getState(m_NativeApi.SessionHandle, lightEstimateHandle, ref state);
            return state.ToLightEstimateState();
        }

        public float GetPixelIntensity(IntPtr lightEstimateHandle)
        {
            float pixelIntensity = 0;
            ExternApi.ArLightEstimate_getPixelIntensity(m_NativeApi.SessionHandle,
                lightEstimateHandle, ref pixelIntensity);
            return pixelIntensity;
        }

        private struct ExternApi
        {
            [DllImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArLightEstimate_create(IntPtr sessionHandle,
                ref IntPtr lightEstimateHandle);

            [DllImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArLightEstimate_destroy(IntPtr lightEstimateHandle);

            [DllImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArLightEstimate_getState(IntPtr sessionHandle,
                IntPtr lightEstimateHandle, ref ApiLightEstimateState state);

            [DllImport(ApiConstants.ARCoreNativeApi)]
            public static extern void ArLightEstimate_getPixelIntensity(IntPtr sessionHandle,
                IntPtr lightEstimateHandle, ref float pixelIntensity);
        }
    }
}
