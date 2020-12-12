export interface EnvironmentConfig {
  readonly authentication: AuthenticationConfig;
  readonly cdn: CdnConfig;
}

export interface AuthenticationConfig {
  readonly baseUri: string;
}

export interface CdnConfig {
  readonly baseUri: string;
}
