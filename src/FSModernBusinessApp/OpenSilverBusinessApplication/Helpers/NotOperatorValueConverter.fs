namespace $ext_safeprojectname$

open System
open System.Globalization
open System.Windows.Data

type NotOperatorValueConverter() =
    interface IValueConverter with
        member this.Convert(value: obj, targetType: Type, parameter: obj, culture: CultureInfo) : obj =
            not (unbox<bool> value) :> obj

        member this.ConvertBack(value: obj, targetType: Type, parameter: obj, culture: CultureInfo) : obj =
            not (unbox<bool> value) :> obj