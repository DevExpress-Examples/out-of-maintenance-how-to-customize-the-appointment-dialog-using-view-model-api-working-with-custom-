Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Linq
Imports System.Web
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports DevExpress.Web
Imports DevExpress.Web.ASPxScheduler.Dialogs
Imports DevExpress.Web.ASPxScheduler.Internal
Imports DevExpress.XtraScheduler

Namespace WebApplication1
    Partial Public Class [Default]
        Inherits System.Web.UI.Page

        Protected Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs)
            Dim dialog = ASPxScheduler1.OptionsForms.DialogLayoutSettings.AppointmentDialog
            dialog.ViewModel = New CustomAppointmentEditDialogViewModel()
            dialog.GenerateDefaultLayoutElements()

            Dim companies = dialog.LayoutElements.CreateField(Function(m As CustomAppointmentEditDialogViewModel) m.AppointmentCompany)
            Dim contacts = dialog.LayoutElements.CreateField(Function(m As CustomAppointmentEditDialogViewModel) m.AppointmentContact)
            dialog.InsertBefore(companies, dialog.FindLayoutElement("Description"))
            dialog.InsertAfter(contacts, companies)

        End Sub


        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs)
        End Sub

        Protected Sub ObjectDataSourceResources_ObjectCreated(ByVal sender As Object, ByVal e As ObjectDataSourceEventArgs)
            If Session("CustomResourceDataSource") Is Nothing Then
                Session("CustomResourceDataSource") = New CustomResourceDataSource(GetCustomResources())
            End If
            e.ObjectInstance = Session("CustomResourceDataSource")
        End Sub

        Private Function GetCustomResources() As BindingList(Of CustomResource)
            Dim resources As New BindingList(Of CustomResource)()
            resources.Add(CreateCustomResource(1, "Max Fowler"))
            resources.Add(CreateCustomResource(2, "Nancy Drewmore"))
            resources.Add(CreateCustomResource(3, "Pak Jang"))
            Return resources
        End Function

        Private Function CreateCustomResource(ByVal res_id As Integer, ByVal caption As String) As CustomResource
            Dim cr As New CustomResource()
            cr.ResID = res_id
            cr.Name = caption
            Return cr
        End Function

        Public RandomInstance As New Random()
        Private Function CreateCustomAppointment(ByVal subject As String, ByVal resourceId As Object, ByVal status As Integer, ByVal label As Integer) As CustomAppointment
            Dim apt As New CustomAppointment()
            apt.Subject = subject
            apt.OwnerId = resourceId
            Dim rnd As Random = RandomInstance
            Dim rangeInMinutes As Integer = 60 * 24
            apt.StartTime = Date.Today + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes))
            apt.EndTime = apt.StartTime.Add(TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes \ 4)))
            apt.Status = status
            apt.Label = label
            Return apt
        End Function

        Protected Sub ObjectDataSourceAppointment_ObjectCreated(ByVal sender As Object, ByVal e As ObjectDataSourceEventArgs)
            If Session("CustomAppointmentDataSource") Is Nothing Then
                Session("CustomAppointmentDataSource") = New CustomAppointmentDataSource(GetCustomAppointments())
            End If
            e.ObjectInstance = Session("CustomAppointmentDataSource")
        End Sub

        Private Function GetCustomAppointments() As BindingList(Of CustomAppointment)
            Dim appointments As New BindingList(Of CustomAppointment)()

            Dim resources As CustomResourceDataSource = TryCast(Session("CustomResourceDataSource"), CustomResourceDataSource)
            If resources IsNot Nothing Then
                For Each item As CustomResource In resources.Resources
                    Dim subjPrefix As String = item.Name & "'s "
                    appointments.Add(CreateCustomAppointment(subjPrefix & "meeting", item.ResID, 2, 5))
                    appointments.Add(CreateCustomAppointment(subjPrefix & "travel", item.ResID, 3, 6))
                    appointments.Add(CreateCustomAppointment(subjPrefix & "phone call", item.ResID, 0, 10))
                Next item
            End If
            Return appointments
        End Function
    End Class

    Public Class CustomAppointmentEditDialogViewModel
        Inherits AppointmentEditDialogViewModel

        <DialogFieldViewSettings(Caption := "Company", EditorType := DialogFieldEditorType.ComboBox)> _
        Public Property AppointmentCompany() As Integer
        <DialogFieldViewSettings(Caption := "Contact", EditorType := DialogFieldEditorType.ComboBox)> _
        Public Property AppointmentContact() As Integer

        Public Overrides Sub Load(ByVal appointmentController As AppointmentFormController)
            MyBase.Load(appointmentController)

            SetEditorTypeFor(Function(m) m.Subject, DialogFieldEditorType.ComboBox)
            SetDataItemsFor(Function(m) m.Subject, Sub(addItemDelegate)
                addItemDelegate("meeting", "meeting")
                addItemDelegate("travel", "travel")
                addItemDelegate("phone call", "phonecall")
            End Sub)


            Dim companies As List(Of Company) = Company.GenerateCompanyDataSource()
            SetDataItemsFor(Function(m As CustomAppointmentEditDialogViewModel) m.AppointmentCompany, Sub(addItemDelegate)
                For Each comp As Company In companies
                    addItemDelegate(comp.CompanyName, comp.CompanyID)
                Next comp
            End Sub)

            SetDataItemsFor(Function(m As CustomAppointmentEditDialogViewModel) m.AppointmentContact, Sub(addItemDelegate)
                Dim contacts As List(Of CompanyContact) = CompanyContact.GenerateContactDataSource().Where(Function(c) c.CompanyID = AppointmentCompany).ToList()
                addItemDelegate("", 0)
                For Each cont As CompanyContact In contacts
                    addItemDelegate(cont.ContactName, cont.ContactID)
                Next cont
            End Sub)

            TrackPropertyChangeFor(Function(m As CustomAppointmentEditDialogViewModel) m.AppointmentCompany, Sub()
                AppointmentContact = 0
            End Sub)

            TrackPropertyChangeFor(Function(m As CustomAppointmentEditDialogViewModel) m.Subject, Sub()
            End Sub)

        End Sub

        Public Overrides Sub SetDialogElementStateConditions()
            MyBase.SetDialogElementStateConditions()
            SetItemVisibilityCondition("Location", False)
            SetItemVisibilityCondition(Function(vm) vm.IsAllDay, False)
            SetItemVisibilityCondition(Function(vm) vm.Reminder, False)
            SetItemVisibilityCondition(Function(vm As CustomAppointmentEditDialogViewModel) vm.AppointmentContact, Subject = "phonecall")
            SetItemVisibilityCondition(Function(vm As CustomAppointmentEditDialogViewModel) vm.AppointmentCompany, Subject = "phonecall")
        End Sub
    End Class
End Namespace