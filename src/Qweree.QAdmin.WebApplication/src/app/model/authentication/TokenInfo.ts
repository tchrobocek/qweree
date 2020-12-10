export class TokenInfo {

  constructor(
    public accessToken: string,
    public refreshToken: string,
    public expiresAt: number,
    public createdAt: string
  ) { }

}
