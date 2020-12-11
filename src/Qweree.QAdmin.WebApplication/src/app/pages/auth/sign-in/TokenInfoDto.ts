export class TokenInfoDto {

  constructor(
    // tslint:disable-next-line:variable-name
    public access_token: string,
    // tslint:disable-next-line:variable-name
    public refresh_token: string,
    // tslint:disable-next-line:variable-name
    public expires_in: number,
    // tslint:disable-next-line:variable-name
    public created_at: Date
  ) { }

}
