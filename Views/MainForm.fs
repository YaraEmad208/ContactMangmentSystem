module MainForm

open System
open System.Drawing
open System.Windows.Forms
open Contact
open ContactValidation
open SearchService

type MainForm() as this =
    inherit Form()

    let headerLabel = new Label(Text = "Contact Manager", Font = new Font("Arial", 16.0f, FontStyle.Bold), Dock = DockStyle.Top, ForeColor = Color.DarkBlue, TextAlign = ContentAlignment.MiddleCenter, Height = 40)
    let searchLabel = new Label(Text = "Search", Top = 50, Left = 20, Width = 50)
    let searchTextBox = new TextBox(Top = 50, Left = 100, Width = 300, Height = 40)
    let searchPanel = new FlowLayoutPanel(Top = 40, Left = 400, Width = 500, Height = 50, FlowDirection = FlowDirection.LeftToRight)
    let detailsGroup = new GroupBox(Text = "Contact Details", Top = 90, Left = 20, Width = 500, Height = 180)
    let nameLabel = new Label(Text = "Name", Top = 30, Left = 20, Width = 100, Parent = detailsGroup)
    let nameTextBox = new TextBox(Top = 30, Left = 140, Width = 300, Parent = detailsGroup)
    let phoneLabel = new Label(Text = "Phone", Top = 70, Left = 20, Width = 100, Parent = detailsGroup)
    let phoneTextBox = new TextBox(Top = 70, Left = 140, Width = 300, Parent = detailsGroup)
    let emailLabel = new Label(Text = "Email", Top = 110, Left = 20, Width = 100, Parent = detailsGroup)
    let emailTextBox = new TextBox(Top = 110, Left = 140, Width = 300, Parent = detailsGroup)
    let actionPanel = new FlowLayoutPanel(Top = 285, Left = 50, Width = 500, Height = 50, FlowDirection = FlowDirection.LeftToRight)
    let addButton = new Button(Text = "➕ Add", Width = 120, Height = 40, BackColor = Color.LightGreen)
    let updateButton = new Button(Text = "✏️ Update", Width = 120, Height = 40, BackColor = Color.LightBlue)
    let deleteButton = new Button(Text = "🗑 Delete", Width = 120, Height = 40, BackColor = Color.LightCoral)
    let contactsGroup = new GroupBox(Text = "Contacts", Top = 340, Left = 20, Width = 500, Height = 300)
    let contactsListBox = new ListBox(Top = 30, Left = 20, Width = 460, Height = 240, Parent = contactsGroup)
    let statusLabel = new Label(Top = 640, Left = 20, Width = 500, ForeColor = Color.Black, TextAlign = ContentAlignment.MiddleCenter)

    let contactsDb = ref ContactRepository.initialDatabase

    let updateContactListBox (filter: string option) (contactsDb: Map<int, Contact>) =
        contactsListBox.Items.Clear()
        let filteredContacts = 
            match filter with
            | Some query -> searchContacts query contactsDb
            | None -> contactsDb
        filteredContacts |> Map.iter (fun _ c -> 
            contactsListBox.Items.Add($"Id: {c.Id}, Name: {c.Name}, Phone: {c.Phone}, Email: {c.Email}") |> ignore)

    let setStatus message isError =
        statusLabel.Text <- message
        statusLabel.ForeColor <- if isError then Color.Red else Color.Green

    let phoneExists phone db =
        db |> Map.exists (fun _ contact -> contact.Phone = phone)

    let emailExists email db =
        db |> Map.exists (fun _ contact -> contact.Email = email)


    do
        this.Text <- "Contact Manager"
        this.Size <- new Size(560, 700)

        actionPanel.Controls.AddRange([| addButton; updateButton; deleteButton |])
        searchPanel.Controls.AddRange([| |])
        this.Controls.AddRange([| headerLabel; searchLabel; detailsGroup; searchTextBox; actionPanel; searchPanel; contactsGroup; statusLabel |])

        addButton.Click.Add(fun _ -> 
            let name = nameTextBox.Text
            let phone = phoneTextBox.Text
            let email = emailTextBox.Text

            match validatePhoneNumber phone with
            | Error msg -> setStatus ("Error: " + msg) true
            | Ok _ -> 
                match validateEmail email with
                | Error msg -> setStatus ("Error: " + msg) true
                | Ok _ -> 
                    if phoneExists phone !contactsDb then
                        setStatus "Error: Phone number already exists!" true
                    elif emailExists email !contactsDb then
                        setStatus "Error: Email already exists!" true
                    else
                        let newId = (Map.count !contactsDb) + 1
                        let contact = { Id = newId; Name = name; Phone = phone; Email = email }

                        contactsDb := ContactRepository.addContact contact !contactsDb
                        setStatus "Contact added successfully!" false
                        updateContactListBox None !contactsDb)

        updateButton.Click.Add(fun _ -> 
            match contactsListBox.SelectedItem with
            | null -> setStatus "Please select a contact to update!" true
            | selected -> 
                let selectedText = selected.ToString()
                let id = selectedText.Split([| ',' |]) |> Array.head |> fun part -> part.Replace("Id: ", "").Trim() |> int
                let name = nameTextBox.Text
                let phone = phoneTextBox.Text
                let email = emailTextBox.Text

                match validatePhoneNumber phone, validateEmail email with
                | Error msg, _ | _, Error msg -> setStatus ("Error: " + msg) true
                | Ok _, Ok _ -> 
                    if phoneExists phone !contactsDb && (phone <> (Map.find id !contactsDb).Phone) then
                        setStatus "Error: Phone number already exists!" true
                    elif emailExists email !contactsDb && (email <> (Map.find id !contactsDb).Email) then
                        setStatus "Error: Email already exists!" true
                    else
                        let updatedContact = { Id = id; Name = name; Phone = phone; Email = email }

                        contactsDb := ContactRepository.updateContact updatedContact !contactsDb
                        setStatus "Contact updated successfully!" false
                        updateContactListBox None !contactsDb)

        deleteButton.Click.Add(fun _ -> 
            match contactsListBox.SelectedItem with
            | null -> setStatus "Please select a contact to delete!" true
            | selected -> 
                let selectedText = selected.ToString()
                let id = selectedText.Split([| ',' |]) |> Array.head |> fun part -> part.Replace("Id: ", "").Trim() |> int
                contactsDb := ContactRepository.deleteContact id !contactsDb
                setStatus "Contact deleted successfully!" false
                updateContactListBox None !contactsDb)

        searchTextBox.TextChanged.Add(fun _ -> 
            let searchTerm = searchTextBox.Text
            updateContactListBox (if String.IsNullOrEmpty searchTerm then None else Some searchTerm) !contactsDb)

        contactsListBox.SelectedIndexChanged.Add(fun _ -> 
            match contactsListBox.SelectedItem with
            | null -> ()
            | selected -> 
                let selectedText = selected.ToString()
                let id = selectedText.Split([| ',' |]) |> Array.head |> fun part -> part.Replace("Id: ", "").Trim() |> int
                match Map.tryFind id !contactsDb with
                | Some contact -> 
                    nameTextBox.Text <- contact.Name
                    phoneTextBox.Text <- contact.Phone
                    emailTextBox.Text <- contact.Email
                | None -> setStatus "Contact not found!" true)

        updateContactListBox None !contactsDb
