﻿using System;
#if NETCOREAPP
using Telerik.Windows.Documents.Extensibility;
#endif

namespace GenerateDocument
{
    internal class Program
    {
        private static void Main()
        {
#if NETCOREAPP
            FontsProviderBase fontsProvider = new FontsProvider();
            FixedExtensibilityManager.FontsProvider = fontsProvider;
#endif
            Console.Write("Choose the format you would like to export to (docx/html/rtf/txt/pdf): ");

            //string input = Console.ReadLine();

            DocumentGenerator generator = new DocumentGenerator();
            generator.SelectedExportFormat = "pdf";
            //if (!string.IsNullOrEmpty(input))
            //{
            //    generator.SelectedExportFormat = input;
            //}

            generator.Generate();

            //Console.Read();
        }
    }
}
