<%@ Control Language="C#" AutoEventWireup="true" Inherits="CMSModules_devtrev_GoogleIntegrations_UI_GoogleCalendarEventsRepeater"  CodeFile="~/CMSModules/devtrev.GoogleIntegrations/UI/GoogleCalendarEventsRepeater.ascx.cs" %>
<%@ Register Src="~/CMSModules/devtrev.GoogleIntegrations/Controls/CalendarEventDataSource.ascx" TagPrefix="devtrev" TagName="CalendarEventDataSource" %>
<devtrev:CalendarEventDataSource runat="server" ID="dsGoogleCalendarEvents" />
<cms:CMSUniView runat="server" ID="uniView" />
