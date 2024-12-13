module MainForm
open System
open System.Drawing
open System.Windows.Forms
open Contact
open ContactValidation
open SearchService

type MainForm() as this =
    inherit Form()
    let headerLabel = new Label(Text = "Contact Manager", Font = new Font("Arial", 16.0f, FontStyle.Bold), Dock = DockStyle.Top, TextAlign = ContentAlignment.MiddleCenter, Height = 40)
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
    let addButton = new Button(Text = "Add", Width = 120, Height = 40)
    let updateButton = new Button(Text = "Update", Width = 120, Height = 40)
    let deleteButton = new Button(Text = "Delete", Width = 120, Height = 40)
    let contactsGroup = new GroupBox(Text = "Contacts", Top = 340, Left = 20, Width = 500, Height = 300)
    let contactsListBox = new ListBox(Top = 30, Left = 20, Width = 460, Height = 240, Parent = contactsGroup)
    let statusLabel = new Label(Top = 640, Left = 20, Width = 500, ForeColor = Color.Black, TextAlign = ContentAlignment.MiddleCenter)
    let mutable contactsDb = ContactRepository.initialDatabase

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
    this.Controls.AddRange([| headerLabel; searchLabel; detailsGroup; searchTextBox ; actionPanel; searchPanel; contactsGroup; statusLabel |])
