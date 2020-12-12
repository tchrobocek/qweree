import {HttpClient} from '@angular/common/http';
import {HealthReport} from '../../model/system/HealthReport';
import {Observable} from 'rxjs';
import {UriHelper} from '../UriHelper';
import {VersionReport} from '../../model/system/VersionReport';
import {catchError} from 'rxjs/operators';

export class SystemAdapterService {

  constructor(private httpClient: HttpClient, private uri: string) {
  }

  healthReport(): Observable<HealthReport> {
    return this.httpClient.get<HealthReport>(UriHelper.getUri(this.uri, '/api/v1/_system/health'))
      .pipe(catchError(e => {throw e; }));
  }

  versionReport(): Observable<VersionReport> {
    return this.httpClient.get<VersionReport>(UriHelper.getUri(this.uri, '/api/v1/_system/version'))
      .pipe(catchError(e => {throw e; }));
  }
}
