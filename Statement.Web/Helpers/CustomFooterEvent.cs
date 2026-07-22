using iTextSharp.text;
using iTextSharp.text.pdf;

namespace Statement.Web.Helpers
{
    public class CustomFooterEvent : PdfPageEventHelper
    {
        private readonly Font _footerFont;

        public CustomFooterEvent()
        {
            _footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8, iTextSharp.text.Font.NORMAL);
        }

        public override void OnEndPage(PdfWriter writer, Document document)
        {
            // Create table for footer
            PdfPTable footerTable = new PdfPTable(1)
            {
                TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin,
                LockedWidth = true
            };

            // Create page number phrase
            //string pageText = $"Page {writer.PageNumber}";
            //string pageText = $"Page {writer.PageNumber} - Guaranty Trust Bank (Ghana) Ltd is a member of the Ghana Deposit Protection Scheme. For further information about the Ghana Deposit Protection Scheme, refer to GDPC’s website: www.gdpc.gov.gh”.";
            string pageText = $"Page {writer.PageNumber} - Guaranty Trust Bank (Ghana) Ltd is a member of the Ghana Deposit Protection Scheme. For further information about the Ghana Deposit Protection Scheme, refer to GDPC’s website: www.gdpc.gov.gh";
            PdfPCell cell = new PdfPCell(new Phrase(pageText, _footerFont))
            {
                Border = PdfPCell.NO_BORDER,
                HorizontalAlignment = Element.ALIGN_CENTER,
                PaddingTop = 5,
                PaddingBottom = 10
            };

            // Add to table and render
            footerTable.AddCell(cell);
            footerTable.WriteSelectedRows(0, -1, document.LeftMargin, document.BottomMargin, writer.DirectContent);
        }
    }
}
