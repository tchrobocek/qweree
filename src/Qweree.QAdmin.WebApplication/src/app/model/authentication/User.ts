export class User {
  constructor(
    public id: string,
    public username: string,
    public roles: string[],
    public createdAt: string,
    public modifiedAt: string
  ) {
  }
}
