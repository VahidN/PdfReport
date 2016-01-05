<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="WebAppTests._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Button ID="btnBrowse" runat="server" Text="Browse" OnClick="btnBrowse_Click" />       

        <asp:Button ID="btnInMemory" runat="server" onclick="btnInMemory_Click" 
            Text="In-memory PDF file" />
            <br />
            <br />
        <a href="UpdatePanelTest.aspx">UpdatePanel Test</a>
    </div>
    </form>
</body>
</html>
