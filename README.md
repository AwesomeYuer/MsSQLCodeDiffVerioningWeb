# MsSQLCodeDiffVerioningWeb

*
-
1.
1.
<details><summary>aaa</summary>
* Install TVM Unity. You can either
    * use `pip3 install mlc-ai-nightly -f https://mlc.ai/wheels` to install the TVM Unity wheel, or
    * follow [TVM¡¯s documentation](https://tvm.apache.org/docs/install/from_source.html) to build from source. **Please use `git checkout origin/unity` to checkout to TVM Unity after git clone.**
* To import, optimize and build the stable diffusion model:
    ```shell
    python3 build.py
    ```
    By default `build.py` takes `apple/m2-gpu` as build target. You can also specify CUDA target via
    ```shell
    python3 build.py --target cuda
    ```
* To deploy the model locally with native GPU runtime:
    ```shell
    python3 deploy.py --prompt "A photo of an astronaut riding a horse on mars."
    ```
    You can substitute the prompt with your own one, and optionally use `--negative-prompt "Your negative prompt"` to specify a negative prompt.
* To deploy the model on web with WebGPU runtime, the last section ¡°Deploy on web¡± of the [walkthrough notebook](https://github.com/mlc-ai/web-stable-diffusion/blob/main/walkthrough.ipynb) has listed the full instructions which you can refer to. We also provide the same list of plain instructions here:

</details>
