module SearchService
open ContactRepository

let searchContacts (searchTerm: string) (contacts: ContactDatabase) =
    contacts
    |> Map.filter (fun _ contact -> 
        contact.Name.Contains(searchTerm) || 
        contact.Phone.Contains(searchTerm) || 
        contact.Email.Contains(searchTerm))
