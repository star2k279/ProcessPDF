//using PdfSharp.Pdf;
//using PdfSharp.Pdf.IO;

using iTextSharp.text;
using iTextSharp.text.pdf;

using System;
using System.IO;

namespace ProcessPDF
{
    class Program
    {
        static void Main(string[] args)
        {


            //******************using PDFSharp => Please uncomment class 'ProcessPDFUsingPDFSharp' below*******************

            //ProcessPDFUsingPDFSharp pdfProcessor = new ProcessPDFUsingPDFSharp();
            //System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            ////pdfProcessor.SplitPDF("e:/PDFPOC/CombinedWavScan.pdf", "e:/");

            //bool result = pdfProcessor.CombinePDFs("E:/Personal/Sahar Docs/Sahar Docs/degrees/MscDegree", "MCSDegree1.pdf");


            //******************using iTextSharp  => Please uncomment class 'ProcessPDFUsingiTextSharp' below*******************

            string pdfFilePath = @"e:/PDFPOC/heavyFile.pdf";
            string outputPath = @"e:/PDFPOC/Splitted/";
            int interval = 10;
            int pageNameSuffix = 0;

            // Intialize a new PdfReader instance with the contents of the source Pdf file:  
            PdfReader reader = new PdfReader(pdfFilePath);

            FileInfo file = new FileInfo(pdfFilePath);
            string pdfFileName = file.Name.Substring(0, file.Name.LastIndexOf(".")) + "-";

            ProcessPDFUsingiTextSharp pdfProcessor = new ProcessPDFUsingiTextSharp();

            //pdfProcessor.SplitAndSaveInterval(pdfFilePath, outputPath, interval, pdfFileName);
            pdfProcessor.CombinePDFFiles("E:/PDFPOC/Splitted", "E:/PDFPOC/Combined/CombinedWAV.pdf");

        }
    }


    //public class ProcessPDFUsingPDFSharp
    //{


    //    public ProcessPDFUsingPDFSharp() { }
    //    /// <summary>
    //    /// Split the pdf into as many pdfs as the pages in the source pdf
    //    /// </summary>
    //    /// <param name="inputPDF"> name of the PDF to split with Path</param>
    //    /// <param name="destPath"> location of the destination folder to put in splitted files</param>
    //    /// <returns></returns>
    //    public bool SplitPDF(string inputPDF, string destPath)
    //    {
    //        if (string.IsNullOrEmpty(inputPDF) || !File.Exists(inputPDF)) { return false; }

    //        string filename = inputPDF;


    //       // Open the file
    //       PdfDocument inputDocument = PdfReader.Open(filename, PdfDocumentOpenMode.Import);

    //        string name = Path.GetFileNameWithoutExtension(filename);

    //        for (int idx = 0; idx < inputDocument.PageCount; idx++)
    //        {
    //            string newFilename = Path.Combine("e:/", String.Format("{0} - Page {1}.pdf", name, idx + 1));
    //            //Create new document
    //            PdfDocument outputDocument = new PdfDocument();
    //            outputDocument.Version = inputDocument.Version;
    //            outputDocument.Info.Title =
    //              String.Format("Page {0} of {1}", idx + 1, inputDocument.Info.Title);
    //            outputDocument.Info.Creator = "PDF Processor";

    //            //Add the page and save it
    //            outputDocument.AddPage(inputDocument.Pages[idx]);
    //            outputDocument.Save(newFilename);
    //        }
    //        return true;
    //    }

    //    public bool CombinePDFs(string sourceFilesPath, string destFileName)
    //    {
    //        if (!Directory.Exists(sourceFilesPath)) { return false; }

    //        //Get the directory
    //        DirectoryInfo sourceDirectory = new DirectoryInfo(sourceFilesPath);

    //        //Get all the pdf files in the source directory
    //        FileInfo[] allPdfiles = sourceDirectory.GetFiles("*.pdf");

    //        Array.Sort(allPdfiles, (s1, s2) => DateTime.Compare(s1.CreationTime, s2.CreationTime));
    //        int totalPages = 0;

    //        //Create new pdf document
    //        PdfDocument outputDocument = new PdfDocument();
    //        outputDocument.Version = 12;
    //        outputDocument.Info.Title = "Combined PDF Documents";
    //        outputDocument.Info.Creator = "PDF Processor";

    //        //Now add all the pages from each source pdf file to the output pdf one by one
    //        foreach (FileInfo pdfFile in allPdfiles)
    //        {
    //            using (PdfDocument doc = PdfReader.Open(pdfFile.FullName, PdfDocumentOpenMode.Import))
    //            {
    //                for (int i = 0; i < doc.Pages.Count; i++)
    //                {
    //                    outputDocument.AddPage(doc.Pages[i]);
    //                    totalPages++;
    //                }

    //                doc.Close();

    //            }
    //        }

    //        outputDocument.Save(Path.Combine(sourceFilesPath, destFileName));
    //        return true;
    //    }
    //}



    public class ProcessPDFUsingiTextSharp
    {


        public void SplitAndSaveInterval(string pdfFilePath, string outputPath, int interval, string pdfFileName)
        {

            using (PdfReader reader = new PdfReader(pdfFilePath))
            {
                interval = reader.NumberOfPages;
                string newPdfFileName = pdfFileName + "{0}";



                for (int pagenumber = 1; pagenumber <= interval; pagenumber++)
                {
                    Document document = new Document();
                    PdfCopy copy = new PdfCopy(document, new FileStream(Path.Combine(outputPath, string.Format(newPdfFileName, pagenumber + ".pdf")), FileMode.Create));
                    document.Open();

                    if (reader.NumberOfPages >= pagenumber)
                    {
                        copy.AddPage(copy.GetImportedPage(reader, pagenumber));
                    }
                    else
                    {
                        break;
                    }

                    document.Close();
                }


            }
        }



        public bool CombinePDFFiles(string sourcePath, string targetFile)
        {
            // Create document object  
            iTextSharp.text.Document PDFdoc = new iTextSharp.text.Document();
            //Get the directory
            DirectoryInfo sourceDirectory = new DirectoryInfo(sourcePath);

            //Get all the pdf files in the source directory
            FileInfo[] allPdfFiles = sourceDirectory.GetFiles("*.pdf");

            Array.Sort(allPdfFiles, (s1, s2) => DateTime.Compare(s1.CreationTime, s2.CreationTime));
            int totalPages = 0;

            // Create a object of FileStream which will be disposed at the end  
            using (System.IO.FileStream myFileStream = new System.IO.FileStream(targetFile, System.IO.FileMode.Create))
            {
                // Create a PDFwriter that listens to the Pdf document  
                iTextSharp.text.pdf.PdfSmartCopy PDFwriter = new iTextSharp.text.pdf.PdfSmartCopy(PDFdoc, myFileStream);
                if (PDFwriter == null)
                {
                    return false;
                }
                // Open the PDFdocument  
                PDFdoc.Open();
                foreach (FileInfo file in allPdfFiles)
                {
                    // Create a PDFreader for a certain PDFdocument  
                    iTextSharp.text.pdf.PdfReader PDFreader = new iTextSharp.text.pdf.PdfReader(file.FullName);
                    PDFreader.ConsolidateNamedDestinations();
                    
                    // Add content  
                    for (int i = 1; i <= PDFreader.NumberOfPages; i++)
                    {
                        iTextSharp.text.pdf.PdfImportedPage page = PDFwriter.GetImportedPage(PDFreader, i);
                        
                        PDFwriter.AddPage(page);
                    }
                    
                    PDFreader.Close();
                }
                // Close the PDFdocument and PDFwriter  
                PDFwriter.Close();
                PDFdoc.Close();

                return true;
            }
        }

    }
}
