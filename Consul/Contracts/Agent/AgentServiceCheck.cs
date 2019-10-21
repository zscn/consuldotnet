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
using System;

namespace Consul.Contracts.Agent
{
    /// <summary>
    /// AgentServiceCheck is used to create an associated check for a service
    /// </summary>
    public class AgentServiceCheck
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Script { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string DockerContainerID { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Shell { get; set; } // Only supported for Docker.

        [JsonConverter(typeof(DurationTimespanConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan? Interval { get; set; }

        [JsonConverter(typeof(DurationTimespanConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan? Timeout { get; set; }

        [JsonConverter(typeof(DurationTimespanConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan? TTL { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string HTTP { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TCP { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(HealthStatusConverter))]
        public HealthStatus Status { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool TLSSkipVerify { get; set; }

        /// <summary>
        /// In Consul 0.7 and later, checks that are associated with a service
        /// may also contain this optional DeregisterCriticalServiceAfter field,
        /// which is a timeout in the same Go time format as Interval and TTL. If
        /// a check is in the critical state for more than this configured value,
        /// then its associated service (and all of its associated checks) will
        /// automatically be deregistered.
        /// </summary>
        [JsonConverter(typeof(DurationTimespanConverter))]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public TimeSpan? DeregisterCriticalServiceAfter { get; set; }
    }
}
