// Copyright 2016 Google Inc. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissioßns and
// limitations under the License.

using UnityEngine;

/// @cond
namespace Gvr.Internal
{
    class EmulatorConfig : MonoBehaviour
    {
        public static EmulatorConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    EmulatorConfig[] configs = (EmulatorConfig[])FindObjectsOfType(typeof(EmulatorConfig));
                    if (configs.Length == 1)
                    {
                        instance = configs[0];
                    }
                    else if (configs.Length > 1)
                    {
                        Debug.LogError(
                            "Multiple PhoneRemote/Config objects in scene. Ignoring all.");
                    }
                }
                if (instance == null)
                {
                    var gameObject = new GameObject("PhoneRemoteConfig");
                    instance = gameObject.AddComponent<EmulatorConfig>();
                    DontDestroyOnLoad(instance);
                }
                return instance;
            }
        }
        private static EmulatorConfig instance = null;

        public enum Mode
        {
            OFF,
            USB,
            WIFI,
        }

        // Set this value to match how the PC is connected to the phone that is
        // streaming gyro, accel, and touch events. Set to OFF if using Wifi instead.
        public Mode PHONE_EVENT_MODE = Mode.USB;

        /*----- Internal Parameters (should not require any changes). -----*/

        // IP address of the phone, when connected to the PC via USB.
        public static readonly string USB_SERVER_IP = "127.0.0.1";

        // IP address of the phone, when connected to the PC via WiFi.
        // public static readonly string WIFI_SERVER_IP = "192.168.1.40";

        //  혜화역 탐앤탐스
        // public static readonly string WIFI_SERVER_IP = "172.30.1.40";

        //  광교 사무실
        // public static readonly string WIFI_SERVER_IP = "192.168.1.51";

        //  혜화역 kings bean
        // public static readonly string WIFI_SERVER_IP = "172.30.1.10";

        //  공덕 서울창업허브
        // public static readonly string WIFI_SERVER_IP = "172.16.7.88";

        //  한양대 흥신소
        // public static readonly string WIFI_SERVER_IP = "192.168.0.32";

        //  한양대 HYU-wlan 4
        // public static readonly string WIFI_SERVER_IP = "172.16.95.184";

        //  왕십리 우리집
        public static readonly string WIFI_SERVER_IP = "192.168.0.3";

        //  부산 탐탐
        // public static readonly string WIFI_SERVER_IP = "192.168.0.16";

        //  부산 김치숙소
        //public static readonly string WIFI_SERVER_IP = "192.168.0.7";

        //  원택 핫스팟
        // public static readonly string WIFI_SERVER_IP = "192.168.43.60";

        //  문조 핫스팟
        // public static readonly string WIFI_SERVER_IP = "192.168.2.8";

        // public static readonly string WIFI_SERVER_IP = "192.168.0.19";


    }
}
/// @endcond
