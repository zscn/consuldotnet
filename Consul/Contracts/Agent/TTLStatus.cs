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

using System;

namespace Consul.Contracts.Agent
{
    /// <summary>
    /// The status of a TTL check
    /// </summary>
    public class TTLStatus : IEquatable<TTLStatus>
    {
        private static readonly TTLStatus passingStatus = new TTLStatus() { Status = "passing", LegacyStatus = "pass" };
        private static readonly TTLStatus warningStatus = new TTLStatus() { Status = "warning", LegacyStatus = "warn" };
        private static readonly TTLStatus criticalStatus = new TTLStatus() { Status = "critical", LegacyStatus = "fail" };

        public string Status { get; private set; }
        internal string LegacyStatus { get; private set; }

        public static TTLStatus Pass
        {
            get { return passingStatus; }
        }

        public static TTLStatus Warn
        {
            get { return warningStatus; }
        }

        public static TTLStatus Critical
        {
            get { return criticalStatus; }
        }

        [Obsolete("Use TTLStatus.Critical instead. This status will be an error in 0.7.0+", true)]
        public static TTLStatus Fail
        {
            get { return criticalStatus; }
        }

        public bool Equals(TTLStatus other)
        {
            return other != null && ReferenceEquals(this, other);
        }

        public override bool Equals(object other)
        {
            // other could be a reference type, the is operator will return false if null
            return other is TTLStatus && Equals(other as TTLStatus);
        }

        public override int GetHashCode()
        {
            return Status.GetHashCode();
        }
    }
}
