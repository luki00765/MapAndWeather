﻿

#pragma checksum "C:\Users\Lukii\Documents\Visual Studio 2013\Projects\MapAndWeather\MapAndWeather\MapAndWeather.WindowsPhone\MainPage.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7DB717E890C24767363E36F451CCEF17"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MapAndWeather
{
    partial class MainPage : global::Windows.UI.Xaml.Controls.Page, global::Windows.UI.Xaml.Markup.IComponentConnector
    {
        [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.Windows.UI.Xaml.Build.Tasks"," 4.0.0.0")]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
 
        public void Connect(int connectionId, object target)
        {
            switch(connectionId)
            {
            case 1:
                #line 35 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Maps.MapControl)(target)).MapTapped += this.MapControl_OnMapTapped;
                 #line default
                 #line hidden
                break;
            case 2:
                #line 36 "..\..\MainPage.xaml"
                ((global::Windows.UI.Xaml.Controls.Primitives.RangeBase)(target)).ValueChanged += this.MySlider_OnValueChanged;
                 #line default
                 #line hidden
                break;
            }
            this._contentLoaded = true;
        }
    }
}


