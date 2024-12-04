<%@ Page Title="" Language="C#" MasterPageFile="~/PrintReports/Blank.Master" AutoEventWireup="true" CodeBehind="ViewReport.aspx.cs" Inherits="SamaaMarketing.PrintReports.ViewReport" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

    <asp:Panel runat="server" ID="pnlrpt">

        <asp:ScriptManager runat="server"></asp:ScriptManager>
        <script type="text/javascript">
            function PrintReport() {
                var printFrame = document.getElementById("PrintFrame");
                printFrame.src = "PrintForm.aspx?tmp=" + new Date().getTime();
            }
        </script>
        <div class="row clearfix">
            <div class="col-md-12">
                <asp:Label ID="lblSuccess" runat="server" class="col-md-12 alert alert-success" Visible="false"></asp:Label>
                <asp:Label ID="lblError" runat="server" class="col-md-12 alert alert-danger" Visible="false"></asp:Label>
            </div>
        </div>
        <div class="col-lg-12">
            <iframe id="frmPrint" name="IframeName" width="500" height="200" runat="server" style="display: none" runat="server"></iframe>
            <%--<iframe id="frmPrint" name="IframeName" width="500" height="200" runat="server" style="display: none" runat="server"></iframe>--%>
            <%--<a href="javascript:void(0);" id="lnkPrint" class="btn btn-danger waves-effect Print btn-sm lnkprint">Print</a>--%>
            <asp:ImageButton ID="btnPrint" CssClass="btn btn-danger waves-effect Print btn-sm" runat="server" OnClick="btnPrint_Click" AlternateText="Pint" />
            <br />
            <center>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" Visible="false" Width="72%" Height="750px"  ClientIDMode="Static"
                ShowPrintButton="true" ShowRefreshButton="true" Font-Names="Segoe UI" Font-Size="8pt" WaitMessageFont-Names="Verdana" 
                WaitMessageFont-Size="14pt" BorderStyle="Groove" ZoomPercent="100" ZoomMode="Percent" >
            </rsweb:ReportViewer>
        </center>
        </div>
        <script>


        //$(document).ready(function () {
        //    if ($.browser.mozilla  ) {
        //        try {
        //            var ControlName = 'ReportViewer1';
        //            var innerScript = '<scr' + 'ipt type="text/javascript">document.getElementById("' + ControlName + '_print").Controller = new ReportViewerHoverButton("' + ControlName + '_print", false, "", "", "", "#ECE9D8", "#DDEEF7", "#99BBE2", "1px #ECE9D8 Solid", "1px #336699 Solid", "1px #336699 Solid");</scr' + 'ipt>';
        //            var innerTbody = '<tbody><tr><td><input type="image" style="border-width: 0px; padding: 2px; height: 16px; width: 16px;" alt="Print" src="/Reserved.ReportViewerWebControl.axd?OpType=Resource&amp;Version=9.0.30729.1&amp;Name=Microsoft.Reporting.WebForms.Icons.Print.gif" title="Print"></td></tr></tbody>';
        //            var innerTable = '<table title="Print" onmouseout="this.Controller.OnNormal();" onmouseover="this.Controller.OnHover();" onclick="PrintFunc(\'' + ControlName + '\'); return false;" id="' + ControlName + '_print" style="border: 1px solid rgb(236, 233, 216); background-color: rgb(236, 233, 216); cursor: default;">' + innerScript + innerTbody + '</table>'
        //            var outerScript = '<scr' + 'ipt type="text/javascript">document.getElementById("' + ControlName + '_print").Controller.OnNormal();</scr' + 'ipt>';
        //            var outerDiv = '<div style="display: inline; font-size: 8pt; height: 30px;" class=" "><table cellspacing="0" cellpadding="0" style="display: inline;"><tbody><tr><td height="28px">' + innerTable + outerScript + '</td></tr></tbody></table></div>';
        //            $("#" + ControlName + " > div > div").append(outerDiv);
        //        }
        //        catch (e) { alert(e); }
        //    }
        //});

        </script>

    </asp:Panel>

</asp:Content>
