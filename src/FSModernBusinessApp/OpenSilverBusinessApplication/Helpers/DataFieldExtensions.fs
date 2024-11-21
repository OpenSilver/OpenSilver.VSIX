namespace $ext_safeprojectname$

open System
open System.Windows
open System.Windows.Controls
open System.Windows.Data

module DataFieldExtensions =

    /// Creates a new Binding object by copying all properties from another Binding object.
    let private createCopy (binding: Binding) =
        if isNull binding then raise (ArgumentNullException(nameof binding))

        let newBinding = Binding()
        newBinding.BindsDirectlyToSource <- binding.BindsDirectlyToSource
        newBinding.Converter <- binding.Converter
        newBinding.ConverterParameter <- binding.ConverterParameter
        newBinding.ConverterCulture <- binding.ConverterCulture
        newBinding.Mode <- binding.Mode
        newBinding.NotifyOnValidationError <- binding.NotifyOnValidationError
        newBinding.Path <- binding.Path
        newBinding.UpdateSourceTrigger <- binding.UpdateSourceTrigger
        newBinding.ValidatesOnExceptions <- binding.ValidatesOnExceptions

        if not (isNull binding.ElementName) then
            newBinding.ElementName <- binding.ElementName
        elif not (isNull binding.RelativeSource) then
            newBinding.RelativeSource <- binding.RelativeSource
        else
            newBinding.Source <- binding.Source

        newBinding

    /// Replaces a DataField's TextBox control with another control and updates the bindings, allowing for a binding setup function.
    let replaceTextBoxWithSetup (field: DataField) (newControl: FrameworkElement) (dataBindingProperty: DependencyProperty) (bindingSetupFunction: Action<Binding> option) =
        if isNull field then raise (ArgumentNullException(nameof field))
        if isNull newControl then raise (ArgumentNullException(nameof newControl))

        // Construct new binding by copying the existing one and applying any setup function.
        let originalBinding = 
            field.Content.GetBindingExpression(TextBox.TextProperty).ParentBinding

        let newBinding = createCopy originalBinding

        bindingSetupFunction |> Option.iter (fun func -> func.Invoke(newBinding))

        // Replace field content with the new control and bind it.
        newControl.SetBinding(dataBindingProperty, newBinding) |> ignore
        field.Content <- newControl

    /// Replaces a DataField's TextBox control with another control and updates the bindings.
    let replaceTextBox (field: DataField) (newControl: FrameworkElement) (dataBindingProperty: DependencyProperty) =
        replaceTextBoxWithSetup field newControl dataBindingProperty None