export class HealthReport {
  constructor(
    public status: string,
    public entries: any
  ) {
    if (!entries) {
      this.entries = [];
    }
  }
}
