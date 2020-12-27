export class TokenInfo {

  constructor(
    public accessToken: string,
    public refreshToken: string,
    public expiresAt: number,
    public createdAt: string
  ) { }

}

export class UserInfo {
  constructor(
    public userId: string,
    public username: string,
    public fullName: string,
    public email: string,
    public role: string[],
  ) {
  }
}
