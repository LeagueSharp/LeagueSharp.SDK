// <copyright file="AssemblyInfo.cs" company="LeagueSharp">
//    Copyright (c) 2015 LeagueSharp.
// 
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
// 
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU General Public License for more details.
// 
//    You should have received a copy of the GNU General Public License
//    along with this program.  If not, see http://www.gnu.org/licenses/
// </copyright>

using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle("LeagueSharp.SDK")]
[assembly: AssemblyDescription("LeagueSharp Software Development Kit")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("LeagueSharp")]
[assembly: AssemblyProduct("LeagueSharp SDK")]
[assembly: AssemblyCopyright("Copyright © LeagueSharp 2015")]
[assembly: AssemblyTrademark("LeagueSharp")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]
[assembly: Guid("e4860c36-20f0-4ca9-ad94-aa71eae94e8e")]

/*
 * Major.Minor.Build.Revision
 * 
 * Revision This is the number taken from source control to identify what was actually built.
 * Build This is an ever increasing number that can be used to find a particular build on the build server. It is an important number because the build server may have built the same source twice with a different set of parameters. Using the build number in conjunction with the source number allows you to identify what was built and how.
 * Minor This should only change when there is a significant change to the public interface. For instance, if it is an API, would consuming code still be able to compile? This number should be reset to zero when the Major number changes.
 * Major indicates what version of the product you are on. For example the Major of all the VisualStudio 2008 assemblies is 9 and VisualStudio 2010 is 10.
*/
[assembly: AssemblyVersion("0.1.0.0")]
[assembly: AssemblyFileVersion("0.1.0.0")]
[assembly: NeutralResourcesLanguage("en")]