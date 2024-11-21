namespace $ext_safeprojectname$

open System
open System.Globalization
open System.Windows.Data

type TargetNullValueConverter() =
    interface IValueConverter with
        member this.Convert(value: obj, targetType: Type, parameter: obj, culture: CultureInfo): obj =
            match value with
            | :? string as strValue when String.IsNullOrEmpty(strValue) -> null
            | _ -> value

        member this.ConvertBack(value: obj, targetType: Type, parameter: obj, culture: CultureInfo): obj =
            if value = null then String.Empty :> obj
            else value