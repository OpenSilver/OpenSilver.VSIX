using System.ComponentModel;

namespace $ext_safeprojectname$.Models
{
    /// <summary>
    /// Extensions to the <see cref="User"/> class.
    /// </summary>
    public partial class User
    {
        #region Make DisplayName Bindable

        protected override void OnPropertyChanged(string propertyName)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == "Name" || propertyName == "FriendlyName")
            {
                this.RaisePropertyChanged("DisplayName");
            }
        }

        #endregion Make DisplayName Bindable
    }
}