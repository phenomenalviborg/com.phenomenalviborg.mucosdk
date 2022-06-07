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
namespace Antilatency.Alt.Environment.Selector {

[Guid("2ef251c9-e0d5-46ad-8ae0-967ded908f2a")]
public interface ILibrary : Antilatency.InterfaceContract.IInterface {
	void registerEnvironmentProvider(string name, Antilatency.Alt.Environment.IEnvironmentConstructor provider);
	void unregisterEnvironmentProvider(string name);
	Antilatency.Alt.Environment.IEnvironment createEnvironment(string data);
}
public static class Library{
    [DllImport("AntilatencyAltEnvironmentSelector")]
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
	public class ILibraryWrapper : Antilatency.InterfaceContract.Details.IInterfaceWrapper, ILibrary {
		private ILibraryRemap.VMT _VMT = new ILibraryRemap.VMT();
		protected new int GetTotalNativeMethodsCount() {
		    return base.GetTotalNativeMethodsCount() + typeof(ILibraryRemap.VMT).GetFields().Length;
		}
		public ILibraryWrapper(System.IntPtr obj) : base(obj) {
		    _VMT = LoadVMT<ILibraryRemap.VMT>(base.GetTotalNativeMethodsCount());
		}
		public void registerEnvironmentProvider(string name, Antilatency.Alt.Environment.IEnvironmentConstructor provider) {
			var nameMarshaler = Antilatency.InterfaceContract.Details.ArrayInMarshaler.create(name);
			var providerMarshaler = Antilatency.InterfaceContract.Details.InterfaceMarshaler.ManagedToNative<Antilatency.Alt.Environment.IEnvironmentConstructor>(provider);
			HandleExceptionCode(_VMT.registerEnvironmentProvider(_object, nameMarshaler, providerMarshaler));
			nameMarshaler.Dispose();
		}
		public void unregisterEnvironmentProvider(string name) {
			var nameMarshaler = Antilatency.InterfaceContract.Details.ArrayInMarshaler.create(name);
			HandleExceptionCode(_VMT.unregisterEnvironmentProvider(_object, nameMarshaler));
			nameMarshaler.Dispose();
		}
		public Antilatency.Alt.Environment.IEnvironment createEnvironment(string data) {
			Antilatency.Alt.Environment.IEnvironment result;
			System.IntPtr resultMarshaler;
			var dataMarshaler = Antilatency.InterfaceContract.Details.ArrayInMarshaler.create(data);
			HandleExceptionCode(_VMT.createEnvironment(_object, dataMarshaler, out resultMarshaler));
			dataMarshaler.Dispose();
			result = (resultMarshaler==System.IntPtr.Zero) ? null : new Antilatency.Alt.Environment.Details.IEnvironmentWrapper(resultMarshaler);
			return result;
		}
	}
	public class ILibraryRemap : Antilatency.InterfaceContract.Details.IInterfaceRemap {
		public new struct VMT {
			public delegate Antilatency.InterfaceContract.ExceptionCode registerEnvironmentProviderDelegate(System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate name, System.IntPtr provider);
			public delegate Antilatency.InterfaceContract.ExceptionCode unregisterEnvironmentProviderDelegate(System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate name);
			public delegate Antilatency.InterfaceContract.ExceptionCode createEnvironmentDelegate(System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate data, out System.IntPtr result);
			#pragma warning disable 0649
			public registerEnvironmentProviderDelegate registerEnvironmentProvider;
			public unregisterEnvironmentProviderDelegate unregisterEnvironmentProvider;
			public createEnvironmentDelegate createEnvironment;
			#pragma warning restore 0649
		}
		public new static readonly NativeInterfaceVmt NativeVmt;
		static ILibraryRemap() {
			var vmtBlocks = new System.Collections.Generic.List<object>();
			AppendVmt(vmtBlocks);
			NativeVmt = new NativeInterfaceVmt(vmtBlocks);
		}
		protected static new void AppendVmt(System.Collections.Generic.List<object> buffer) {
			Antilatency.InterfaceContract.Details.IInterfaceRemap.AppendVmt(buffer);
			var vmt = new VMT();
			vmt.registerEnvironmentProvider = (System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate name, System.IntPtr provider) => {
				try {
					var obj = GetContext(_this) as ILibrary;
					var providerMarshaler = provider == System.IntPtr.Zero ? null : new Antilatency.Alt.Environment.Details.IEnvironmentConstructorWrapper(provider);
					obj.registerEnvironmentProvider(name, providerMarshaler);
				}
				catch (System.Exception ex) {
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			vmt.unregisterEnvironmentProvider = (System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate name) => {
				try {
					var obj = GetContext(_this) as ILibrary;
					obj.unregisterEnvironmentProvider(name);
				}
				catch (System.Exception ex) {
					return handleRemapException(ex, _this);
				}
				return Antilatency.InterfaceContract.ExceptionCode.Ok;
			};
			vmt.createEnvironment = (System.IntPtr _this, Antilatency.InterfaceContract.Details.ArrayInMarshaler.Intermediate data, out System.IntPtr result) => {
				try {
					var obj = GetContext(_this) as ILibrary;
					var resultMarshaler = obj.createEnvironment(data);
					result = Antilatency.InterfaceContract.Details.InterfaceMarshaler.ManagedToNative<Antilatency.Alt.Environment.IEnvironment>(resultMarshaler);
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
