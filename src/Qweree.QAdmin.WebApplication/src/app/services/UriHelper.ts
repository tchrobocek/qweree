export class UriHelper {
  static getUri(base: string, path: string): string {
    let uri = base;

    if (base.endsWith('/')) {
      uri = uri.substring(0, base.length - 1);
    }

    if (!path) {
      return uri;
    }

    if (!path.startsWith('/')) {
      uri += '/';
    }

    return uri + path;
  }
}
