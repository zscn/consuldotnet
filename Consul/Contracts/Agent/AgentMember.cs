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

using System.Collections.Generic;

namespace Consul.Contracts.Agent
{
    /// <summary>
    /// AgentMember represents a cluster member known to the agent
    /// </summary>
    public class AgentMember
    {
        public string Name { get; set; }
        public string Addr { get; set; }
        public ushort Port { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public int Status { get; set; }
        public byte ProtocolMin { get; set; }
        public byte ProtocolMax { get; set; }
        public byte ProtocolCur { get; set; }
        public byte DelegateMin { get; set; }
        public byte DelegateMax { get; set; }
        public byte DelegateCur { get; set; }

        public AgentMember()
        {
            Tags = new Dictionary<string, string>();
        }
    }
}
