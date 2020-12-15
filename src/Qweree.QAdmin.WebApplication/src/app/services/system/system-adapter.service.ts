import {HttpClient} from '@angular/common/http';
import {HealthReport} from '../../model/system/HealthReport';
import {Observable} from 'rxjs';
import {PathHelper} from '../PathHelper';
import {VersionReport} from '../../model/system/VersionReport';
import {catchError} from 'rxjs/operators';

export class SystemAdapterService {

  constructor(private httpClient: HttpClient, private uri: string) {
  }

  healthReport(): Observable<HealthReport> {
    return this.httpClient.get<HealthReport>(PathHelper.getPath(this.uri, '/api/v1/_system/health'))
      .pipe(catchError(e => {throw e; }));
  }

  versionReport(): Observable<VersionReport> {
    return this.httpClient.get<VersionReport>(PathHelper.getPath(this.uri, '/api/v1/_system/version'))
      .pipe(catchError(e => {throw e; }));
  }
}
