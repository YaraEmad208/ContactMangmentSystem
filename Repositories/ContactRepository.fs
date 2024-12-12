module ContactRepository
open Contact

type ContactDatabase = Map<int, Contact>
let initialDatabase: ContactDatabase = Map.empty

let getAllContacts (contacts: ContactDatabase): ContactDatabase =
    contacts

let addContact (contact: Contact) (contacts: ContactDatabase): ContactDatabase =
    contacts |> Map.add contact.Id contact

let updateContact (contact: Contact) (contacts: ContactDatabase): ContactDatabase =
    contacts |> Map.add contact.Id contact

let deleteContact (id: int) (contacts: ContactDatabase): ContactDatabase =
    contacts |> Map.remove id