/*
* Copyright (c) 2023 PlayEveryWare
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in all
* copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
* SOFTWARE.
*/
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;

namespace PlayEveryWare.EpicOnlineServices.Samples.Network
{
    [InitializeOnLoad]
    public class PackageInstallHelper_PostProcessing
    {
        static PackageInstallHelper_PostProcessing()
        {

#if !COM_UNITY_MODULE_POSTPROCESSING
            AddRequest request;
            Debug.LogWarning("Package : [com.unity.postprocessing] required, attempting to install...");

            request = Client.Add("com.unity.postprocessing@3.2.2");
            while (request.Status == StatusCode.InProgress) { }
            if (request.Result != null) { Debug.Log("[com.unity.postprocessing@3.2.2] successfully installed"); }
            else { Debug.Log("[com.unity.postprocessing@3.2.2] Request Failed : " + request.Error.ToString()); }
#endif

#if !COM_UNITY_MODULE_SHADERGRAPH
            AddRequest request2;
            Debug.LogWarning("Package : [com.unity.shadergraph] required, attempting to install...");

            request2 = Client.Add("com.unity.shadergraph@12.1.7");
            while (request2.Status == StatusCode.InProgress) { }
            if (request2.Result != null) { Debug.Log("[com.unity.shadergraph@12.1.7] successfully installed"); }
            else { Debug.Log("[com.unity.shadergraph@12.1.7] Request Failed : " + request2.Error.ToString()); }
#endif
        }
    }
}
#endif

