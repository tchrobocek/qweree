import { Injectable } from '@angular/core';
import { environment} from '../../../environments/environment';
import {EnvironmentConfig} from '../authentication/EnvironmentConfig';

@Injectable({
  providedIn: 'root'
})
export class EnvironmentService {

  private readonly environmentConfig: EnvironmentConfig;

  constructor() {
    this.environmentConfig = {
      authentication: {
        baseUri: environment.authentication.baseUri
      },
      cdn: {
        baseUri: environment.authentication.baseUri
      }
    };
  }

  getEnvironment(): EnvironmentConfig{
    return this.environmentConfig;
  }
}
