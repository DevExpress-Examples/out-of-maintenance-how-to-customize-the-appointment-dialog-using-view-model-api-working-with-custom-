using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.ASPxScheduler.Dialogs;
using DevExpress.Web.ASPxScheduler.Internal;
using DevExpress.XtraScheduler;

namespace WebApplication1 {
    public partial class Default : System.Web.UI.Page {
        protected void Page_Init(object sender, EventArgs e) {
            var dialog = ASPxScheduler1.OptionsForms.DialogLayoutSettings.AppointmentDialog;
            dialog.ViewModel = new CustomAppointmentEditDialogViewModel();
            dialog.GenerateDefaultLayoutElements();

            var companies = dialog.LayoutElements.CreateField((CustomAppointmentEditDialogViewModel m) => m.AppointmentCompany);
            var contacts = dialog.LayoutElements.CreateField((CustomAppointmentEditDialogViewModel m) => m.AppointmentContact);
            dialog.InsertBefore(companies, dialog.FindLayoutElement("Description"));
            dialog.InsertAfter(contacts, companies);

        }


        protected void Page_Load(object sender, EventArgs e) {
        }

        protected void ObjectDataSourceResources_ObjectCreated(object sender, ObjectDataSourceEventArgs e) {
            if(Session["CustomResourceDataSource"] == null) {
                Session["CustomResourceDataSource"] = new CustomResourceDataSource(GetCustomResources());
            }
            e.ObjectInstance = Session["CustomResourceDataSource"];
        }

        BindingList<CustomResource> GetCustomResources() {
            BindingList<CustomResource> resources = new BindingList<CustomResource>();
            resources.Add(CreateCustomResource(1, "Max Fowler"));
            resources.Add(CreateCustomResource(2, "Nancy Drewmore"));
            resources.Add(CreateCustomResource(3, "Pak Jang"));
            return resources;
        }

        private CustomResource CreateCustomResource(int res_id, string caption) {
            CustomResource cr = new CustomResource();
            cr.ResID = res_id;
            cr.Name = caption;
            return cr;
        }

        public Random RandomInstance = new Random();
        private CustomAppointment CreateCustomAppointment(string subject, object resourceId, int status, int label) {
            CustomAppointment apt = new CustomAppointment();
            apt.Subject = subject;
            apt.OwnerId = resourceId;
            Random rnd = RandomInstance;
            int rangeInMinutes = 60 * 24;
            apt.StartTime = DateTime.Today + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes));
            apt.EndTime = apt.StartTime + TimeSpan.FromMinutes(rnd.Next(0, rangeInMinutes / 4));
            apt.Status = status;
            apt.Label = label;
            return apt;
        }

        protected void ObjectDataSourceAppointment_ObjectCreated(object sender, ObjectDataSourceEventArgs e) {
            if(Session["CustomAppointmentDataSource"] == null) {
                Session["CustomAppointmentDataSource"] = new CustomAppointmentDataSource(GetCustomAppointments());
            }
            e.ObjectInstance = Session["CustomAppointmentDataSource"];
        }

        BindingList<CustomAppointment> GetCustomAppointments() {
            BindingList<CustomAppointment> appointments = new BindingList<CustomAppointment>(); ;
            CustomResourceDataSource resources = Session["CustomResourceDataSource"] as CustomResourceDataSource;
            if(resources != null) {
                foreach(CustomResource item in resources.Resources) {
                    string subjPrefix = item.Name + "'s ";
                    appointments.Add(CreateCustomAppointment(subjPrefix + "meeting", item.ResID, 2, 5));
                    appointments.Add(CreateCustomAppointment(subjPrefix + "travel", item.ResID, 3, 6));
                    appointments.Add(CreateCustomAppointment(subjPrefix + "phone call", item.ResID, 0, 10));
                }
            }
            return appointments;
        }
    }

    public class CustomAppointmentEditDialogViewModel : AppointmentEditDialogViewModel { 
        [DialogFieldViewSettings(Caption = "Company", EditorType = DialogFieldEditorType.ComboBox)]
        public int AppointmentCompany { get; set; }
        [DialogFieldViewSettings(Caption = "Contact", EditorType = DialogFieldEditorType.ComboBox)]
        public int AppointmentContact { get; set; }

        public override void Load(AppointmentFormController appointmentController) {
            base.Load(appointmentController);

            SetEditorTypeFor(m => m.Subject, DialogFieldEditorType.ComboBox);
            SetDataItemsFor(m => m.Subject, (addItemDelegate) => {
                addItemDelegate("meeting", "meeting");
                addItemDelegate("travel", "travel");
                addItemDelegate("phone call", "phonecall");
            });


            List<Company> companies = Company.GenerateCompanyDataSource();
            SetDataItemsFor((CustomAppointmentEditDialogViewModel m) => m.AppointmentCompany, (addItemDelegate) => {
                foreach(Company comp in companies) {
                    addItemDelegate(comp.CompanyName, comp.CompanyID);
                }
            });

            SetDataItemsFor((CustomAppointmentEditDialogViewModel m) => m.AppointmentContact, (addItemDelegate) => {
                List<CompanyContact> contacts = CompanyContact.GenerateContactDataSource().Where(c => c.CompanyID == AppointmentCompany).ToList();
                addItemDelegate("", 0);
                foreach(CompanyContact cont in contacts) {
                    addItemDelegate(cont.ContactName, cont.ContactID);
                }
            });

            TrackPropertyChangeFor((CustomAppointmentEditDialogViewModel m) => m.AppointmentCompany, () => {
                AppointmentContact = 0;
            });

            TrackPropertyChangeFor((CustomAppointmentEditDialogViewModel m) => m.Subject, () => {
                
            });            
            
        }

        public override void SetDialogElementStateConditions() {
            base.SetDialogElementStateConditions();
            SetItemVisibilityCondition("Location", false);
            SetItemVisibilityCondition(vm => vm.IsAllDay, false);
            SetItemVisibilityCondition(vm => vm.Reminder, false);
            SetItemVisibilityCondition((CustomAppointmentEditDialogViewModel vm) => vm.AppointmentContact, Subject == "phonecall");
            SetItemVisibilityCondition((CustomAppointmentEditDialogViewModel vm) => vm.AppointmentCompany, Subject == "phonecall");
        }
    }
}