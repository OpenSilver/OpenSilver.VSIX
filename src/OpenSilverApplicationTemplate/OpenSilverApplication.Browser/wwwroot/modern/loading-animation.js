(function () {
    function startLoader() {
        const count = document.querySelector(".opensilver-odometer");
        const loader = document.querySelector(".opensilver-loader-progress-bar");

        if (!count || !loader) return;

        loader.style.width = "0%";

        const observer = new MutationObserver(updateCount);
        function updateCount() {
            const loadPercentageText = getComputedStyle(document.documentElement)
                .getPropertyValue("--blazor-load-percentage-text")
                .trim();
            const loadPercentage = parseInt(loadPercentageText.replace(/"/g, ""));
            const currentValue = isNaN(loadPercentage) ? 0 : loadPercentage;

            animateCounter(currentValue);
            loader.style.width = currentValue + "%";

            if (currentValue === 100) {
                observer.disconnect();
                return;
            }
        }

        observer.observe(document.documentElement, {
            attributes: true,
            attributeFilter: ["style"],
        });

        updateCount();
    }

    function animateCounter(newValue) {
        newValue = Math.min(newValue, 100);

        const count = Array.from(document.querySelectorAll(".opensilver-odometer"));
        const currentValue = count.map((span) => span.textContent).join("");
        const newValueString = String(newValue).padStart(3, "0");
        for (let i = 0; i < newValueString.length; i++) {
            if (newValueString[i] !== currentValue[i]) {
                gsap.to(count[i], {
                    y: -4,
                    opacity: 0.5,
                    duration: 0.075,
                    ease: "none",
                    onComplete: () => {
                        count[i].textContent = newValueString[i];
                        gsap.fromTo(
                            count[i],
                            { y: 4, opacity: 0.5 },
                            { y: 0, opacity: 1, duration: 0.075, ease: "none" }
                        );
                    },
                });
            }
        }

        if (newValue === 100 && currentValue !== "100") {
            gsap.to(count[1], {
                y: -4,
                opacity: 0.5,
                duration: 0.075,
                ease: "none",
                onComplete: () => {
                    count[1].textContent = "0";
                    gsap.fromTo(
                        count[1],
                        { y: 4, opacity: 0.5 },
                        { y: 0, opacity: 1, duration: 0.075, ease: "none" }
                    );
                },
            });

            gsap.to(count[2], {
                y: -4,
                opacity: 0.5,
                duration: 0.075,
                ease: "none",
                onComplete: () => {
                    count[2].textContent = "0";
                    gsap.fromTo(
                        count[2],
                        { y: 4, opacity: 0.5 },
                        { y: 0, opacity: 1, duration: 0.075, ease: "none" }
                    );
                },
            });
        }
    }

    function onDomReady() {
        startLoader();

        function startAnimations() {
            gsap.to(".opensilver-loader-progress", {
                width: "60vw",
                opacity: 1,
                duration: 1.25,
                ease: "power1.out",
                delay: 0.4,
            });

            gsap.to(".opensilver-counter-container", {
                opacity: 1,
                duration: 0.3,
                ease: "none",
                delay: 1.1,
            });

            gsap.to(".opensilver-counter-container > .opensilver-odometer", {
                transform: "translateY(0)",
                duration: 0.3,
                ease: "none",
                delay: 1.1,
            });
        }

        startAnimations();
    }

    const script = document.createElement('script');
    script.setAttribute('type', 'application/javascript');
    script.setAttribute('src', 'https://cdnjs.cloudflare.com/ajax/libs/gsap/3.12.5/gsap.min.js');
    script.addEventListener('load', function () {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', onDomReady);
        } else {
            onDomReady();
        }
    });
    document.head.appendChild(script);
})();