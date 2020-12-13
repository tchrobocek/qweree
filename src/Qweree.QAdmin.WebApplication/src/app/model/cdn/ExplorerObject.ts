export interface ExplorerObject {
  readonly filename: string;
  readonly path: string;
}

export class ExplorerDirectory implements ExplorerObject {
  constructor(
    public filename: string,
    public path: string,
    public totalCount: number,
    public totalSize: number,
    public createdAt: string,
    public modifiedAt: string,
  ) {
  }
}

export class ExplorerFile implements ExplorerObject {
  constructor(
    public filename: string,
    public path: string,
    public mediaType: string,
    public size: number,
    public createdAt: string,
    public modifiedAt: string,
    ) {
  }
}
