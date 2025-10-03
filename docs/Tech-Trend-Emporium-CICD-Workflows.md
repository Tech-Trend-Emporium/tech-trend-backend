# CI/CD Workflow Diagrams

This README documents the three GitHub Actions workflows using Mermaid diagrams. It includes:
- A flowchart for ci-merge-dev, ci-pr-dev-open and cd-merge-main.
- A high-level sequence diagram spanning CI and CD workflow
- An simple end-to-end integration and deployment flow

## Flowcharts
### `ci-pr-dev-open.yml`

```mermaid
flowchart TD
    trig_0(["Trigger: Open PR on develop"])
    validate{"Job: validate"}
    subgraph validate_steps[Steps for validate]
        validate_s0["Checkout"]
        validate_s1["Set variables"]
        validate_s0 --> validate_s1
        validate_s2["Setup .NET"]
        validate_s1 --> validate_s2
        validate_s3["Restore"]
        validate_s2 --> validate_s3
        validate_s4["Build (Release)"]
        validate_s3 --> validate_s4
        validate_s5["Discover test projects"]
        validate_s4 --> validate_s5
        validate_s6["Run tests if any"]
        validate_s5 --> validate_s6
        validate_s7["Summarize"]
        validate_s6 --> validate_s7
    end
    validate --> validate_s0
    trig_0 --> validate
```


### `ci-merge-dev.yml`

```mermaid
flowchart TD
    trig_0(["Trigger: merge on develop"])
    build_and_push{"Job: build-and-push"}
    subgraph build_and_push_steps[Steps for build-and-push]
        build_and_push_s0["Checkout"]
        build_and_push_s1["Set variables"]
        build_and_push_s0 --> build_and_push_s1
        build_and_push_s2["Setup .NET"]
        build_and_push_s1 --> build_and_push_s2
        build_and_push_s3["Restore"]
        build_and_push_s2 --> build_and_push_s3
        build_and_push_s4["Build (Release)"]
        build_and_push_s3 --> build_and_push_s4
        build_and_push_s5["Discover test projects"]
        build_and_push_s4 --> build_and_push_s5
        build_and_push_s6["Run tests (if any)"]
        build_and_push_s5 --> build_and_push_s6
        build_and_push_s7["Docker metadata (tags + labels)"]
        build_and_push_s6 --> build_and_push_s7
        build_and_push_s8["Azure login (OIDC)"]
        build_and_push_s7 --> build_and_push_s8
        build_and_push_s9["ACR docker login via az"]
        build_and_push_s8 --> build_and_push_s9
        build_and_push_s10["Setup Buildx"]
        build_and_push_s9 --> build_and_push_s10
        build_and_push_s11["Build & Push"]
        build_and_push_s10 --> build_and_push_s11
        build_and_push_s12["Capture digest"]
        build_and_push_s11 --> build_and_push_s12
        build_and_push_s13["Publish image reference (for CD)"]
        build_and_push_s12 --> build_and_push_s13
        build_and_push_s14["Summary"]
        build_and_push_s13 --> build_and_push_s14
    end
    build_and_push --> build_and_push_s0
    trig_0 --> build_and_push
```

### `cd-merge-main.yml`

```mermaid
flowchart TD
    trig_0(["Trigger: merge on main"])
    deploy{"Job: deploy"}
    subgraph deploy_steps[Steps for deploy]
        deploy_s0["Azure login"]
        deploy_s1["Install Azure CLI extensions"]
        deploy_s0 --> deploy_s1
        deploy_s2["ACR login + derive repo path"]
        deploy_s1 --> deploy_s2
        deploy_s3["Resolve tag or accept digest"]
        deploy_s2 --> deploy_s3
        deploy_s4["Ensure multi-revision (blue/green) mode"]
        deploy_s3 --> deploy_s4
        deploy_s5["Update image → create new revision"]
        deploy_s4 --> deploy_s5
        deploy_s6["Short wait (no health endpoint yet)"]
        deploy_s5 --> deploy_s6
        deploy_s7["Shift 100% traffic to new revision"]
        deploy_s6 --> deploy_s7
        deploy_s8["Output"]
        deploy_s7 --> deploy_s8
    end
    deploy --> deploy_s0
    trig_0 --> deploy
```

## Cross-Workflow Sequence
```mermaid
sequenceDiagram
    autonumber
    participant Dev as Developer
    participant GH as GitHub
    participant GHA as GitHub Actions
    participant ACR as Azure Container Registry
    participant ACA as Azure Container Apps
    participant Mon as Health Check

    Dev->>GH: Open PR to develop
    GHA->>GHA: Restore, Build, Test (no ACR push)
    GHA->>GH: Report status checks

    Dev->>GH: Push to development
    GH->>GHA: Trigger CI (ci-merge-dev.yml)
    GHA->>GHA: Restore, Build, Test
    GHA->>ACR: Push image (with tag + digest)
    GHA->>GH: Echo the tags of the img

    Dev->>GH: Merge PR to main
    GH->>GHA: Trigger CD (cd-merge-main.yml)
    GHA->>ACR: Pull image by digest
    GHA->>ACA: Deploy revision (blue/green)
    ACA->>Mon: Expose /health
    Mon-->>GHA: Liveness/EF checks OK
    GHA->>GH: Post deployment status
```

## End-to-End Flow
```mermaid
    flowchart LR
    A[Developer Opens PR to 'develop'] --> B[CI Triggered GitHub Actions]
    B --> C[Restore, Build, Test no ACR push]
    C --> D[Report status checks to GitHub]

    D --> E[Developer Pushes to 'develop']
    E --> F[CI Triggered via ci-merge-dev.yml]
    F --> G[Restore, Build, Test]
    G --> H[Push image to ACR tag + digest]
    H --> I[Echo image tags to GitHub]

    I --> J[Developer Merges PR to 'main']
    J --> K[CD Triggered via cd-merge-main.yml]
    K --> L[Pull image from ACR by digest]
    L --> M[Deploy to Azure Container Apps Blue/Green]
    M --> N[Expose /health endpoint]

    N --> O[Health Check Service Performs Liveness/EF Checks]
    O --> P[Post Deployment Status to GitHub]

    %% Optional styling
    classDef ci fill:#d0f0fd,stroke:#007acc,stroke-width:2px;
    classDef cd fill:#d9fcd4,stroke:#22863a,stroke-width:2px;
    class B,C,D,F,G,H,I ci;
    class K,L,M,N,O,P cd;
```

---

> Generated with the help of ChatGPT