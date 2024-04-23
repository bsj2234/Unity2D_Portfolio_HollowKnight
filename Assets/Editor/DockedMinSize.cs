using System.Reflection;
using UnityEditor;
using UnityEngine;
 
namespace Kogane.Internal
{
    [InitializeOnLoad]
    internal static class SetDockedMinSize
    {
        static SetDockedMinSize()
        {
            // https://github.com/Unity-Technologies/UnityCsReference/blob/bf25390e5c79172c3d3e9a6b755680679e1dbd50/Editor/Mono/HostView.cs#L94
            var type      = typeof( Editor ).Assembly.GetType( "UnityEditor.HostView" );
            var fieldInfo = type.GetField( "k_DockedMinSize", BindingFlags.Static | BindingFlags.NonPublic );
 
            fieldInfo!.SetValue( null, new Vector2( 100, 44 ) );
        }
    }
}