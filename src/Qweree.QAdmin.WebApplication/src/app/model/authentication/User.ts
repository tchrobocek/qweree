export class User {
  constructor(
    public id: string,
    public username: string,
    public fullName: string|null,
    public email: string|null,
    public roles: string[],
    public createdAt: string,
    public modifiedAt: string
  ) {
  }
}
