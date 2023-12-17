export interface SignUpRequest 
{
    Email: string;
    Password: string;
}

export interface SignInRequest extends SignUpRequest {}

export interface ConfirmSignUpRequest 
{
    Email: string;
    ConfirmationCode: string;
}
