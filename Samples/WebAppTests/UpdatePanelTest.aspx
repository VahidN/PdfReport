<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UpdatePanelTest.aspx.cs"
    Inherits="WebAppTests.UpdatePanelTest" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Button ID="btnInMemory" runat="server" Text="In-memory PDF file" OnClick="btnInMemory_Click" />
            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnInMemory" />
            </Triggers>
        </asp:UpdatePanel>
    </div>
    </form>
</body>
</html>
