﻿#pragma checksum "..\..\CriarCategoriaWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "698E642ED4230062A5E50DE1FDA04182805440EC"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using JoãoBarbosa_PAP;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace JoãoBarbosa_PAP {
    
    
    /// <summary>
    /// CriarCategoriaWindow
    /// </summary>
    public partial class CriarCategoriaWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 11 "..\..\CriarCategoriaWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox NomeCategoriaTxt;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\CriarCategoriaWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CriarCategoriaBtn;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\CriarCategoriaWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CancelarCategoriaBtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/JoãoBarbosa_PAP;component/criarcategoriawindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\CriarCategoriaWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.NomeCategoriaTxt = ((System.Windows.Controls.TextBox)(target));
            return;
            case 2:
            this.CriarCategoriaBtn = ((System.Windows.Controls.Button)(target));
            
            #line 12 "..\..\CriarCategoriaWindow.xaml"
            this.CriarCategoriaBtn.Click += new System.Windows.RoutedEventHandler(this.CriarCategoriaBtn_Click);
            
            #line default
            #line hidden
            return;
            case 3:
            this.CancelarCategoriaBtn = ((System.Windows.Controls.Button)(target));
            
            #line 13 "..\..\CriarCategoriaWindow.xaml"
            this.CancelarCategoriaBtn.Click += new System.Windows.RoutedEventHandler(this.CancelarCategoriaBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

