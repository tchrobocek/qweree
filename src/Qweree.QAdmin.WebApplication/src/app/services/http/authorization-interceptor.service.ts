import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import {TokenStorageService} from '../authentication/token-storage.service';
import {EnvironmentService} from '../environment/environment.service';

@Injectable()
export class AuthorizationInterceptor implements HttpInterceptor {

  constructor(
    private environmentService: EnvironmentService,
    private tokenStorageService: TokenStorageService
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    if (request.url.startsWith(this.environmentService.getEnvironment().cdn.baseUri) ||
      request.url.startsWith(this.environmentService.getEnvironment().authentication.baseUri)) {
      const tokenInfo = this.tokenStorageService?.getTokenInfo();
      if (tokenInfo) {
        request = request.clone({
          setHeaders: {
            Authorization: 'Bearer ' + tokenInfo.accessToken
          }
        });
      }
    }

    return next.handle(request);
  }
}
