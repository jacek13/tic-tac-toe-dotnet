export interface SignUpResponse
{
    UserId: string;
    Email: string;
    Message: string;
}

export interface SignInResponse
{
    IdToken: string;
    Token: string;
    ExpiresIn: number;
    RefreshToken: string;
}