Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Web

Namespace WebApplication1
    Public Class CustomAppointment
        Private m_Start As Date
        Private m_End As Date
        Private m_Subject As String
        Private m_Status As Integer
        Private m_Description As String
        Private m_Label As Integer
        Private m_Location As String
        Private m_Allday As Boolean
        Private m_EventType As Integer
        Private m_RecurrenceInfo As String
        Private m_ReminderInfo As String
        Private m_OwnerId As Object
        Private m_Id As Object

        Private m_company_id As Integer
        Private m_conact_id As Integer


        Public Property StartTime() As Date
            Get
                Return m_Start
            End Get
            Set(ByVal value As Date)
                m_Start = value
            End Set
        End Property
        Public Property EndTime() As Date
            Get
                Return m_End
            End Get
            Set(ByVal value As Date)
                m_End = value
            End Set
        End Property
        Public Property Subject() As String
            Get
                Return m_Subject
            End Get
            Set(ByVal value As String)
                m_Subject = value
            End Set
        End Property
        Public Property Status() As Integer
            Get
                Return m_Status
            End Get
            Set(ByVal value As Integer)
                m_Status = value
            End Set
        End Property
        Public Property Description() As String
            Get
                Return m_Description
            End Get
            Set(ByVal value As String)
                m_Description = value
            End Set
        End Property
        Public Property Label() As Integer
            Get
                Return m_Label
            End Get
            Set(ByVal value As Integer)
                m_Label = value
            End Set
        End Property
        Public Property Location() As String
            Get
                Return m_Location
            End Get
            Set(ByVal value As String)
                m_Location = value
            End Set
        End Property
        Public Property AllDay() As Boolean
            Get
                Return m_Allday
            End Get
            Set(ByVal value As Boolean)
                m_Allday = value
            End Set
        End Property
        Public Property EventType() As Integer
            Get
                Return m_EventType
            End Get
            Set(ByVal value As Integer)
                m_EventType = value
            End Set
        End Property
        Public Property RecurrenceInfo() As String
            Get
                Return m_RecurrenceInfo
            End Get
            Set(ByVal value As String)
                m_RecurrenceInfo = value
            End Set
        End Property
        Public Property ReminderInfo() As String
            Get
                Return m_ReminderInfo
            End Get
            Set(ByVal value As String)
                m_ReminderInfo = value
            End Set
        End Property
        Public Property OwnerId() As Object
            Get
                Return m_OwnerId
            End Get
            Set(ByVal value As Object)
                m_OwnerId = value
            End Set
        End Property
        Public Property Id() As Object
            Get
                Return m_Id
            End Get
            Set(ByVal value As Object)
                m_Id = value
            End Set
        End Property

        Public Property CompanyID() As Integer
            Get
                Return m_company_id
            End Get
            Set(ByVal value As Integer)
                m_company_id = value
            End Set
        End Property
        Public Property ContactID() As Integer
            Get
                Return m_conact_id
            End Get
            Set(ByVal value As Integer)
                m_conact_id = value
            End Set
        End Property

        Public Sub New()
        End Sub
    End Class

    Public Class CustomResource
        Private m_name As String
        Private m_res_id As Integer

        Public Property Name() As String
            Get
                Return m_name
            End Get
            Set(ByVal value As String)
                m_name = value
            End Set
        End Property
        Public Property ResID() As Integer
            Get
                Return m_res_id
            End Get
            Set(ByVal value As Integer)
                m_res_id = value
            End Set
        End Property

        Public Sub New()
        End Sub
    End Class

    Public Class Company
        Private m_company_name As String
        Private m_company_id As Integer

        Public Property CompanyName() As String
            Get
                Return m_company_name
            End Get
            Set(ByVal value As String)
                m_company_name = value
            End Set
        End Property
        Public Property CompanyID() As Integer
            Get
                Return m_company_id
            End Get
            Set(ByVal value As Integer)
                m_company_id = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Shared Function GenerateCompanyDataSource() As List(Of Company)
            Dim returnedResult As New List(Of Company)()
            For i As Integer = 0 To 9
                returnedResult.Add(New Company() With { _
                    .CompanyID = i, _
                    .CompanyName = "Company " & i.ToString() _
                })
            Next i
            Return returnedResult
        End Function
    End Class

    Public Class CompanyContact
        Private m_contact_name As String
        Private m_contact_id As Integer
        Private m_company_id As Integer

        Public Property ContactName() As String
            Get
                Return m_contact_name
            End Get
            Set(ByVal value As String)
                m_contact_name = value
            End Set
        End Property
        Public Property ContactID() As Integer
            Get
                Return m_contact_id
            End Get
            Set(ByVal value As Integer)
                m_contact_id = value
            End Set
        End Property
        Public Property CompanyID() As Integer
            Get
                Return m_company_id
            End Get
            Set(ByVal value As Integer)
                m_company_id = value
            End Set
        End Property

        Public Sub New()
        End Sub

        Public Shared Function GenerateContactDataSource() As List(Of CompanyContact)
            Dim returnedResult As New List(Of CompanyContact)()
            Dim companies As List(Of Company) = Company.GenerateCompanyDataSource()

            Dim uniqueContactID As Integer = 0
            For i As Integer = 0 To companies.Count - 1
                For j As Integer = 0 To 4
                    returnedResult.Add(New CompanyContact() With { _
                        .CompanyID = i, _
                        .ContactName = "Contact " & j.ToString() & ", Company " & i.ToString(), _
                        .ContactID = uniqueContactID _
                    })
                    uniqueContactID += 1
                Next j
            Next i
            Return returnedResult
        End Function
    End Class
End Namespace