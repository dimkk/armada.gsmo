<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DocumentSetUserControl.ascx.cs" Inherits="SAMRT.WebParts.DocumentSetUserControl, SAMRT.WebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=3f3087b80f01a99d" %>

<a style='padding: 20px;' href='<%=DocumentSetLink%>' target='_blank' mscui:controltype='Button' role='button' unselectable='on'>
    <div style='display: table; overflow: hidden;'>
        <div style='display: table-cell;'>
            <span class='ms-cui-img-32by32 ms-cui-img-cont-float ms-cui-imageDisabled' unselectable='on'>
                <img unselectable='on' src='/_layouts/images/docset_captureversion_32.png?rev=23'>
            </span>
        </div>
        <div style='display: table-cell; padding-left: 5px; vertical-align: middle;'>
            <span unselectable='on'>Открыть материалы в отдельном окне</span>
        </div>
    </div>
</a>