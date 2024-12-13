module ContactService
open Contact
open ContactRepository
open OperationResult
open ContactValidation

let addContact (contact: Contact) (contacts: ContactDatabase) =
    match validatePhoneNumber contact.Phone with
    | Error msg -> Failure (InvalidPhoneNumber msg), contacts
    | Ok _ ->
        match validateEmail contact.Email with
        | Error msg -> Failure (InvalidEmail msg), contacts
        | Ok _ -> 
            Success contact, contacts.Add(contact.Id, contact)

let updateContact (contact: Contact) (contacts: ContactDatabase) =
    match validatePhoneNumber contact.Phone with
    | Error msg -> Failure (InvalidPhoneNumber msg), contacts
    | Ok _ ->
        match validateEmail contact.Email with
        | Error msg -> Failure (InvalidEmail msg), contacts
        | Ok _ -> 
            Success contact, contacts.Add(contact.Id, contact)

let deleteContact (id: int) (contacts: ContactDatabase) =
    if contacts.ContainsKey(id) then
        Success id, contacts.Remove(id)
    else
        Failure (ContactNotFound ("Contact not found with ID: " + string id)), contacts

let getAllContacts (contacts: ContactDatabase) =
    Success (contacts)