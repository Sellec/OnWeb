﻿using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Общие сведения об этой сборке предоставляются следующим набором
// набора атрибутов. Измените значения этих атрибутов, чтобы изменить сведения,
// связанные со сборкой.
[assembly: AssemblyTitle("OnWeb.CoreBind")]
[assembly: AssemblyDescription("Ядро веб-движка для работы с ASP.NET MVC.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("TraceStudio")]
[assembly: AssemblyProduct("OnWeb")]
[assembly: AssemblyCopyright("Copyright © Петров Дмитрий 2016 (Sellec)")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Установка значения False для параметра ComVisible делает типы в этой сборке невидимыми
// для компонентов COM. Если необходимо обратиться к типу в этой сборке через
// COM, задайте атрибуту ComVisible значение TRUE для этого типа.
[assembly: ComVisible(false)]

// Следующий GUID служит для идентификации библиотеки типов, если этот проект будет видимым для COM
[assembly: Guid("97e67e63-4e52-48b7-93d3-775b3ffac10d")]

// Сведения о версии сборки состоят из следующих четырех значений:
//
//      Основной номер версии
//      Дополнительный номер версии
//   Номер сборки
//      Редакция
//
// Можно задать все значения или принять номер сборки и номер редакции по умолчанию.
// используя "*", как показано ниже:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("0.1.4.1")]
[assembly: AssemblyFileVersion("0.1.4.1")]
[assembly: System.Web.PreApplicationStartMethod(typeof(OnWeb.CoreBind.Razor.WebStartup), nameof(OnWeb.CoreBind.Razor.WebStartup.PreApplicationStartMethod))]
