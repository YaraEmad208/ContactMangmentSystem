module ContactValidation
open System.Text.RegularExpressions
let validatePhoneNumber (phone: string) =
    let pattern = @"^\d{3}-\d{3}-\d{4}$"  
    if Regex.IsMatch(phone, pattern) then
        Ok phone
    else
        Error ("Invalid phone number format")

let validateEmail (email: string) =
    let pattern = @"^[^@]+@[^@]+\.[^@]+$"  
    if Regex.IsMatch(email, pattern) then
        Ok email
    else
        Error ("Invalid email format")

