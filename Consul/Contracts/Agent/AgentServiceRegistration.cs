// -----------------------------------------------------------------------
//  <copyright file="Agent.cs" company="PlayFab Inc">
//    Copyright 2015 PlayFab Inc.
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Consul.Contracts.Agent
{
    /// <summary>
    /// AgentServiceRegistration is used to register a new service
    /// </summary>
    public class AgentServiceRegistration
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string ID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] Tags { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int Port { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Address { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool EnableTagOverride { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AgentServiceCheck Check { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AgentServiceCheck[] Checks { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Meta { get; set; }
    }
}
