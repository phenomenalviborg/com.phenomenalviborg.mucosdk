//Copyright 2021, ALT LLC. All Rights Reserved.
//This file is part of Antilatency SDK.
//It is subject to the license terms in the LICENSE file found in the top-level directory
//of this distribution and at http://www.antilatency.com/eula
//You may not use this file except in compliance with the License.
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
#pragma warning disable IDE1006 // Do not warn about naming style violations
#pragma warning disable IDE0017 // Do not suggest to simplify object initialization
using System.Runtime.InteropServices; //GuidAttribute
namespace Antilatency.Alt.Environment.Pillars {

[System.Serializable]
[System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
public partial struct PillarData {
	public UnityEngine.Vector3 position;
	public uint kind;
}

[Guid("50d5c3b4-88a5-4dec-9d20-2d0d8f6505bc")]
public interface IDeserializedEnvironment : Antilatency.InterfaceContract.IInterface {
	Antilatency.Alt.Environment.Pillars.PillarData[] getPillars();
	float[] getProportions();
	uint getNumMarkersPerPillar();
	float getPillarLength();
}
namespace Details {
	public class IDeserializedEnvironmentWrapper : Antilatency.InterfaceContract.Details.IInterfaceWrapper, IDeserializedEnvironment {
		private IDeserializedEnvironmentRemap.VMT _VMT = new IDeserializedEnvironmentRemap.VMT();
		protected new int GetTotalNativeMethodsCount() {
		    return base.GetTotalNativeMethodsCount() + typeof(IDeserializedEnvironmentRemap.VMT).GetFields().Length;
		}
		public IDeserializedEnvironmentWrapper(System.IntPtr obj) : base(obj) {
		    _VMT = LoadVMT<IDeserializedEnvironmentRemap.VMT>(base.GetTotalNativeMethodsCount());
		}
		public Antilatency.Alt.Environment.Pillars.PillarData[] getPillars() {
			Antilatency.Alt.Environment.Pillars.PillarData[] result;
			var resultMarshaler = Antilatency.InterfaceContract.Details.ArrayOutMarshaler.create<Antilatency.Alt.Environment.Pillars.PillarData>();
			HandleExceptionCode(_VMT.getPillars(_object, resultMarshaler));
			result = resultMarshaler.value;
			resultMarshaler.Dispose();
			return result;
		}
		public float[] getProportions() {
			float[] result;
			var resultMarshaler = Antilatency.InterfaceContract.Details.ArrayOutMarshaler.create<float>();
			HandleExceptionCode(_VMT.getProportions(_object, resultMarshaler));
			result = resultMarshaler.value;
			resultMarshaler.Dispose();
			return result;
		}
		public uint getNumMarkersPerPillar() {
			uint result;
			uint resultMarshaler;
			HandleExceptionCode(_VMT.getNumMarkersPerPillar(_object, out resultMarshaler));
			result = resultMarshaler;
			return result;
		}
		public float getPillarLength() {
			float result;
			float resultMarshaler;
			HandleExceptionCode(_VMT.getPillarLength(_object, out resultMarshaler));
			result = resultMarshaler;
			return result;
		}
	}
	public class IDeserializedEnvironmentRemap : Antilatency.InterfaceContract.Details.IInterfaceRemap {
		public new struct VMT {
			public delegate Antilatency.InterfaceContract.ExceptionCode getPillarsDelegate(System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayOutMarshaler.Intermediate result);
			public delegate Antilatency.InterfaceContract.ExceptionCode getProportionsDelegate(System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayOutMarshaler.Intermediate result);
			public delegate Antilatency.InterfaceContract.ExceptionCode getNumMarkersPerPillarDelegate(System.IntPtr _this, out uint result);
			public delegate Antilatency.InterfaceContract.ExceptionCode getPillarLengthDelegate(System.IntPtr _this, out float result);
			#pragma warning disable 0649
			public getPillarsDelegate getPillars;
			public getProportionsDelegate getProportions;
			public getNumMarkersPerPillarDelegate getNumMarkersPerPillar;
			public getPillarLengthDelegate getPillarLength;
			#pragma warning restore 0649
		}
		public new static readonly NativeInterfaceVmt NativeVmt;
		static IDeserializedEnvironmentRemap() {
			var vmtBlocks = new System.Collections.Generic.List<object>();
			AppendVmt(vmtBlocks);
			NativeVmt = new NativeInterfaceVmt(vmtBlocks);
		}
		protected static new void AppendVmt(System.Collections.Generic.List<object> buffer) {
			Antilatency.InterfaceContract.Details.IInterfaceRemap.AppendVmt(buffer);
			var vmt = new VMT();
			vmt.getPillars = (System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayOutMarshaler.Intermediate result) => {
				try {
					var obj = GetContext(_this) as IDeserializedEnvironment;
					var resultMarshaler = obj.getPillars();
					result.assign(resultMarshaler);
				}
				catch (System.Exception ex) {
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			vmt.getProportions = (System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayOutMarshaler.Intermediate result) => {
				try {
					var obj = GetContext(_this) as IDeserializedEnvironment;
					var resultMarshaler = obj.getProportions();
					result.assign(resultMarshaler);
				}
				catch (System.Exception ex) {
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			vmt.getNumMarkersPerPillar = (System.IntPtr _this, out uint result) => {
				try {
					var obj = GetContext(_this) as IDeserializedEnvironment;
					var resultMarshaler = obj.getNumMarkersPerPillar();
					result = resultMarshaler;
				}
				catch (System.Exception ex) {
					result = default(uint);
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			vmt.getPillarLength = (System.IntPtr _this, out float result) => {
				try {
					var obj = GetContext(_this) as IDeserializedEnvironment;
					var resultMarshaler = obj.getPillarLength();
					result = resultMarshaler;
				}
				catch (System.Exception ex) {
					result = default(float);
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			buffer.Add(vmt);
		}
		public IDeserializedEnvironmentRemap() { }
		public IDeserializedEnvironmentRemap(System.IntPtr context, ushort lifetimeId) {
			AllocateNativeInterface(NativeVmt.Handle, context, lifetimeId);
		}
	}
}

[Guid("fb691fa6-ccd5-4b82-aad9-13d70a8b8322")]
public interface ILibrary : Antilatency.Alt.Environment.IEnvironmentConstructor {
	string serialize(Antilatency.Alt.Environment.Pillars.PillarData[] pillars, float[] proportions, uint numMarkersPerPillar, float pillarLength);
	Antilatency.Alt.Environment.Pillars.IDeserializedEnvironment deserialize(string environmentData);
}
public static class Library{
    [DllImport("AntilatencyAltEnvironmentPillars")]
    private static extern Antilatency.InterfaceContract.ExceptionCode getLibraryInterface(System.IntPtr unloader, out System.IntPtr result);
    public static ILibrary load(){
        System.IntPtr libraryAsIInterfaceIntermediate;
        getLibraryInterface(System.IntPtr.Zero, out libraryAsIInterfaceIntermediate);
        Antilatency.InterfaceContract.IInterface libraryAsIInterface = new Antilatency.InterfaceContract.Details.IInterfaceWrapper(libraryAsIInterfaceIntermediate);
        var library = libraryAsIInterface.QueryInterface<ILibrary>();
        libraryAsIInterface.Dispose();
        return library;
    }
}
namespace Details {
	public class ILibraryWrapper : Antilatency.Alt.Environment.Details.IEnvironmentConstructorWrapper, ILibrary {
		private ILibraryRemap.VMT _VMT = new ILibraryRemap.VMT();
		protected new int GetTotalNativeMethodsCount() {
		    return base.GetTotalNativeMethodsCount() + typeof(ILibraryRemap.VMT).GetFields().Length;
		}
		public ILibraryWrapper(System.IntPtr obj) : base(obj) {
		    _VMT = LoadVMT<ILibraryRemap.VMT>(base.GetTotalNativeMethodsCount());
		}
		public string serialize(Antilatency.Alt.Environment.Pillars.PillarData[] pillars, float[] proportions, uint numMarkersPerPillar, float pillarLength) {
			string result;
			var resultMarshaler = Antilatency.InterfaceContract.Details.ArrayOutMarshaler.create();
			var pillarsMarshaler = Antilatency.InterfaceContract.Details.ArrayInMarshaler.create(pillars);
			var proportionsMarshaler = Antilatency.InterfaceContract.Details.ArrayInMarshaler.create(proportions);
			HandleExceptionCode(_VMT.serialize(_object, pillarsMarshaler, proportionsMarshaler, numMarkersPerPillar, pillarLength, resultMarshaler));
			pillarsMarshaler.Dispose();
			proportionsMarshaler.Dispose();
			result = resultMarshaler.value;
			resultMarshaler.Dispose();
			return result;
		}
		public Antilatency.Alt.Environment.Pillars.IDeserializedEnvironment deserialize(string environmentData) {
			Antilatency.Alt.Environment.Pillars.IDeserializedEnvironment result;
			System.IntPtr resultMarshaler;
			var environmentDataMarshaler = Antilatency.InterfaceContract.Details.ArrayInMarshaler.create(environmentData);
			HandleExceptionCode(_VMT.deserialize(_object, environmentDataMarshaler, out resultMarshaler));
			environmentDataMarshaler.Dispose();
			result = (resultMarshaler==System.IntPtr.Zero) ? null : new Antilatency.Alt.Environment.Pillars.Details.IDeserializedEnvironmentWrapper(resultMarshaler);
			return result;
		}
	}
	public class ILibraryRemap : Antilatency.Alt.Environment.Details.IEnvironmentConstructorRemap {
		public new struct VMT {
			public delegate Antilatency.InterfaceContract.ExceptionCode serializeDelegate(System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate pillars, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate proportions, uint numMarkersPerPillar, float pillarLength, Antilatency.InterfaceContract.Details.ArrayOutMarshaler.Intermediate result);
			public delegate Antilatency.InterfaceContract.ExceptionCode deserializeDelegate(System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate environmentData, out System.IntPtr result);
			#pragma warning disable 0649
			public serializeDelegate serialize;
			public deserializeDelegate deserialize;
			#pragma warning restore 0649
		}
		public new static readonly NativeInterfaceVmt NativeVmt;
		static ILibraryRemap() {
			var vmtBlocks = new System.Collections.Generic.List<object>();
			AppendVmt(vmtBlocks);
			NativeVmt = new NativeInterfaceVmt(vmtBlocks);
		}
		protected static new void AppendVmt(System.Collections.Generic.List<object> buffer) {
			Antilatency.Alt.Environment.Details.IEnvironmentConstructorRemap.AppendVmt(buffer);
			var vmt = new VMT();
			vmt.serialize = (System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate pillars, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate proportions, uint numMarkersPerPillar, float pillarLength, Antilatency.InterfaceContract.Details.ArrayOutMarshaler.Intermediate result) => {
				try {
					var obj = GetContext(_this) as ILibrary;
					var resultMarshaler = obj.serialize(pillars.toArray<Antilatency.Alt.Environment.Pillars.PillarData>(), proportions.toArray<float>(), numMarkersPerPillar, pillarLength);
					result.assign(resultMarshaler);
				}
				catch (System.Exception ex) {
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			vmt.deserialize = (System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate environmentData, out System.IntPtr result) => {
				try {
					var obj = GetContext(_this) as ILibrary;
					var resultMarshaler = obj.deserialize(environmentData);
					result = Antilatency.InterfaceContract.Details.InterfaceMarshaler.ManagedToNative<Antilatency.Alt.Environment.Pillars.IDeserializedEnvironment>(resultMarshaler);
				}
				catch (System.Exception ex) {
					result = default(System.IntPtr);
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			buffer.Add(vmt);
		}
		public ILibraryRemap() { }
		public ILibraryRemap(System.IntPtr context, ushort lifetimeId) {
			AllocateNativeInterface(NativeVmt.Handle, context, lifetimeId);
		}
	}
}


}
