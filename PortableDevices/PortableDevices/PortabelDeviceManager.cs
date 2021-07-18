using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
//
// Defined in header file as follows 
// IPortableDeviceManager : public IUnknown
//    {
//    public:
//        virtual HRESULT STDMETHODCALLTYPE GetDevices( 
//            /* [unique][out][in] */
    //      __RPC__deref_opt_inout_opt LPWSTR *pPnPDeviceIDs,
//          /* [out][in] */ __RPC__inout DWORD *pcPnPDeviceIDs) = 0;
        
//        virtual HRESULT STDMETHODCALLTYPE RefreshDeviceList( void) = 0;
        
//        virtual HRESULT STDMETHODCALLTYPE GetDeviceFriendlyName( 
//            /* [in] */ __RPC__in LPCWSTR pszPnPDeviceID,
//            /* [unique][out][in] */ __RPC__inout_opt WCHAR *pDeviceFriendlyName,
//            /* [out][in] */ __RPC__inout DWORD *pcchDeviceFriendlyName) = 0;
        
//        virtual HRESULT STDMETHODCALLTYPE GetDeviceDescription( 
//            /* [in] */ __RPC__in LPCWSTR pszPnPDeviceID,
//            /* [unique][out][in] */ __RPC__inout_opt WCHAR *pDeviceDescription,
//            /* [out][in] */ __RPC__inout DWORD *pcchDeviceDescription) = 0;
        
//        virtual HRESULT STDMETHODCALLTYPE GetDeviceManufacturer( 
//            /* [in] */ __RPC__in LPCWSTR pszPnPDeviceID,
//            /* [unique][out][in] */ __RPC__inout_opt WCHAR *pDeviceManufacturer,
//            /* [out][in] */ __RPC__inout DWORD *pcchDeviceManufacturer) = 0;
        
//        virtual HRESULT STDMETHODCALLTYPE GetDeviceProperty( 
//            /* [in] */ __RPC__in LPCWSTR pszPnPDeviceID,
//            /* [in] */ __RPC__in LPCWSTR pszDevicePropertyName,
//            /* [unique][out][in] */ __RPC__inout_opt BYTE *pData,
//            /* [unique][out][in] */ __RPC__inout_opt DWORD *pcbData,
//            /* [unique][out][in] */ __RPC__inout_opt DWORD *pdwType) = 0;
        
//        virtual HRESULT STDMETHODCALLTYPE GetPrivateDevices( 
//            /* [unique][out][in] */ __RPC__deref_opt_inout_opt LPWSTR *pPnPDeviceIDs,
//            /* [out][in] */ __RPC__inout DWORD *pcPnPDeviceIDs) = 0;
        
//    };

namespace PortableDevices
{
    [ComImport, System.Security.SuppressUnmanagedCodeSecurity,
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("a1567595-4c2f-4574-a6fa-ecef917b9a40")]
    public interface IPortableDeviceManager
    {
        [PreserveSig]
        uint GetDevices(
        [In][MarshalAs(UnmanagedType.LPArray)] IntPtr[] pPnPDeviceIDs,
        [In, Out] ref uint pcPnPDeviceIDs);


        [PreserveSig]
        uint RefreshDeviceList();

        [PreserveSig]
        uint GetDeviceFriendlyName(
            [In] string pszPnPDeviceID,
            [In, Out][MarshalAs(UnmanagedType.LPArray)] char[] pDeviceFriendlyName,
            [In, Out] ref uint pcchDeviceFriendlyName);

        [PreserveSig]
        uint GetDeviceDescription(
            [In] string pszPnpDeviceID,
            [In, Out][MarshalAs(UnmanagedType.LPArray)] char[] pDeviceDescription,
            [In, Out] ref uint pcchDeviceDescription);

        [PreserveSig]
        uint GetDeviceManufacturer(
            [In] string pszPnPDeviceID,
            [In, Out][MarshalAs(UnmanagedType.LPArray)] char[] pDeviceManufacturer,
            [In, Out] ref uint pcchDeviceManufacturer);

        [PreserveSig]
        uint GetDeviceProperty(
            [In] string pszPnPDeviceID,
            [In] string pszDevicePropertyName,
            [In, Out] IntPtr pData,
            [In, Out] ref uint pcbData,
            [In, Out] ref uint pdwType);

      

        [PreserveSig]
        uint GetPrivateDevices(
            [In, Out] IntPtr pPnPDeviceIDs,
            [In, Out] ref uint pcPnPDeviceIDs);

    }

    /// <summary>
    /// CLSID_PortableDeviceManager
    /// </summary>
    [ComImport, Guid("0af10cec-2ecd-4b92-9581-34f6ae0637f3")]
    public class CLSID_PortableDeviceManager { }
}
