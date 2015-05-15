﻿using NBitcoin.Protocol.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NBitcoin.Protocol
{
	public class NodeConnectionParameters
	{

		public NodeConnectionParameters()
		{
			TemplateBehaviors.Add(new PingPongBehavior());
			Version = ProtocolVersion.PROTOCOL_VERSION;
			IsRelay = true;
			Services = NodeServices.Nothing;
			ConnectCancellation = default(CancellationToken);
			ReceiveBufferSize = 1000 * 5000;
			SendBufferSize = 1000 * 1000;
			UserAgent = VersionPayload.GetNBitcoinUserAgent();
		}

		public NodeConnectionParameters(NodeConnectionParameters other)
		{
			Version = other.Version;
			IsRelay = other.IsRelay;
			Services = other.Services;
			ReceiveBufferSize = other.ReceiveBufferSize;
			SendBufferSize = other.SendBufferSize;
			ConnectCancellation = other.ConnectCancellation;
			UserAgent = other.UserAgent;
			AddressFrom = other.AddressFrom;
			IsTrusted = other.IsTrusted;
			Nonce = other.Nonce;
			Advertize = other.Advertize;

			foreach(var behavior in other.TemplateBehaviors)
			{
				TemplateBehaviors.Add((NodeBehavior)((ICloneable)behavior).Clone());
			}
		}

		/// <summary>
		/// Send addr unsollicited message of the AddressFrom peer when passing to Handshaked state
		/// </summary>
		public bool Advertize
		{
			get;
			set;
		}
		public ProtocolVersion Version
		{
			get;
			set;
		}

		/// <summary>
		/// If true, the node will receive all incoming transactions if no bloomfilter are set
		/// </summary>
		public bool IsRelay
		{
			get;
			set;
		}

		public NodeServices Services
		{
			get;
			set;
		}
		/// <summary>
		/// If true, then no proof of work is checked on incoming headers, if null, will trust localhost
		/// </summary>
		public bool? IsTrusted
		{
			get;
			set;
		}
		public string UserAgent
		{
			get;
			set;
		}
		public int ReceiveBufferSize
		{
			get;
			set;
		}
		public int SendBufferSize
		{
			get;
			set;
		}

		public CancellationToken ConnectCancellation
		{
			get;
			set;
		}

		private readonly BehaviorsCollection _TemplateBehaviors = new BehaviorsCollection(null);
		public BehaviorsCollection TemplateBehaviors
		{
			get
			{
				return _TemplateBehaviors;
			}
		}

		public NodeConnectionParameters Clone()
		{
			return new NodeConnectionParameters(this);
		}

		public IPEndPoint AddressFrom
		{
			get;
			set;
		}

		public ulong? Nonce
		{
			get;
			set;
		}

		public VersionPayload CreateVersion(IPEndPoint peer, Network network)
		{
			VersionPayload version = new VersionPayload()
			{
				Nonce = Nonce == null ? RandomUtils.GetUInt64() : Nonce.Value,
				UserAgent = UserAgent,
				Version = Version,
				Timestamp = DateTimeOffset.UtcNow,
				AddressReceiver = peer,
				AddressFrom = AddressFrom ?? new IPEndPoint(IPAddress.Parse("0.0.0.0").MapToIPv6(), network.DefaultPort),
				Relay = IsRelay,
				Services = Services
			};
			return version;
		}
	}
}