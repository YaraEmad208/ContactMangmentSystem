module OperationResult
type ValidationError = 
    | InvalidPhoneNumber of string
    | InvalidEmail of string
    | ContactNotFound of string

type OperationResult<'T> =
    | Success of 'T
    | Failure of ValidationError
